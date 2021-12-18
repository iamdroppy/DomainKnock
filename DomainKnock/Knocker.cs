using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace DomainKnock;

/// <summary>
/// Part 1: Manages the scan
/// </summary>
internal class Knocker
{
    private readonly ILogger<Knocker> _logger;
    private readonly CommandOptions _opts;
    private readonly InitialTcpConnection _initialTcpConnection;
    private readonly HttpHandler _handler;

    public Knocker(ILogger<Knocker> logger, CommandOptions opts, InitialTcpConnection initialTcpConnection, HttpHandler handler)
    {
        _logger = logger;
        _opts = opts;
        _initialTcpConnection = initialTcpConnection;
        _handler = handler;
    }
    public async Task StartAsync()
    {
        _logger.LogInformation($"Knocker starting at {DateTime.Now}");
        _logger.LogTrace("Validating information...");
        if (!IPAddress.TryParse(_opts.Origin, out var startIp))
            throw new Exception("Invalid start ip");
        if (!IPAddress.TryParse(_opts.Destination, out var endIp))
            throw new Exception("Invalid end ip.");

        var sip = startIp.ToReadableFormat();
        var eip = endIp.ToReadableFormat();

        if (sip > eip)
            (sip, eip) = (eip, sip);

        Stopwatch watcher = new Stopwatch();
        watcher.Start();
        _logger.LogInformation($"Scanning {(1 + eip - sip)} IP Addresses");

        uint ipIndex = 0;
        uint scanned = 0;
#pragma warning disable CS4014
        // this is a small "cron" to watch over progress.
        CancellationTokenSource watcherToken = new();
        Task.Run(async () =>
        {
            while (!watcherToken.IsCancellationRequested)
            {
                // ReSharper disable AccessToModifiedClosure
                var currentIpIndex = ipIndex;

                if (currentIpIndex == 0) continue;
                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(_opts.ProgressDelay), watcherToken.Token);
                    _logger.LogDebug($"Scanned {scanned} hosts - elapsed {watcher.Elapsed}");
                }
                catch (OperationCanceledException){ }
                // ReSharper enable AccessToModifiedClosure
            }
        });
#pragma warning restore CS4014

        PortList http = _opts.HttpPorts;
        PortList https = _opts.HttpsPorts;

        if (!http && !https)
        {
            throw new ArgumentException(
                "You must specify the argument --http-ports and/or --https-ports. Use --help for more information.");
        }

        if (http)
            _logger.LogTrace("Http Ports: " + http);
        if (https)
            _logger.LogTrace("Https Ports: " + https);

        for (ipIndex = sip, scanned = 0; ipIndex <= eip; ipIndex++, scanned++)
        {
            IPAddress address = new(BitConverter.GetBytes(ipIndex).Reverse().ToArray());
            var addressStr = address.ToString();
            if (addressStr.EndsWith(".0") || addressStr.EndsWith(".255")) continue;

            if (http)
            {
                foreach (var port in http)
                {
                    try
                    {
                        _logger.LogTrace($"{address.Prefix(port)} Searching for host {_opts.Hostname}.");
                        await ScanAsync(address, false, port);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogTrace($"{address.Prefix(port)} Failed to scan {addressStr}: {ex.Message}", ex);
                    }
                }
            }

            if (https)
            {
                foreach (var port in https)
                {
                    try
                    {
                        _logger.LogTrace($"{address.Prefix(port)} Searching for host {_opts.Hostname}.");
                        await ScanAsync(address, true, port);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogTrace($"{address.Prefix(port)} Failed to scan {addressStr}: {ex.Message}", ex);
                    }
                }
            }
        }

        watcherToken.Cancel(); 
        watcher.Stop();
        _logger.LogInformation($"Took {watcher.Elapsed.ToString()} to complete scan.");
    }

    private async Task ScanAsync(IPAddress address, bool isHttps, ushort port)
    {
        CancellationTokenSource source = new CancellationTokenSource(TimeSpan.FromSeconds(_opts.TimeoutSeconds));
        var client = await _initialTcpConnection.ConnectAsync(address, port, source.Token);
        Stream stream = client.GetStream();
        if (isHttps)
        {
            var ssl = new SslStream(stream, false, (sender, certificate, chain, errors) => true);
            await ssl.AuthenticateAsClientAsync(_opts.Hostname);
            stream = ssl;
        }

        await _handler.RequestAsync(stream, isHttps, address, port, source.Token);
    }
}