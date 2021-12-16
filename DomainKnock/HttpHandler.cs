using System.Net;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;

namespace DomainKnock;

/// <summary>
/// Part 3: Once connected, and the correct stream passed, this will manage all HTTP protocol.
/// </summary>
internal class HttpHandler
{
    private readonly ILogger<HttpHandler> _logger;
    private readonly CommandOptions _opts;

    public HttpHandler(ILogger<HttpHandler> logger, CommandOptions opts)
    {
        _logger = logger;
        _opts = opts;
    }

    public async Task RequestAsync(Stream stream, bool isHttps, IPAddress address, ushort port, CancellationToken token = default)
    {
        using var reader = new StreamReader(stream, leaveOpen: true);
        await using var writer = new StreamWriter(stream, leaveOpen: true);

        _logger.LogTrace($"{address.Prefix(port)} Streams opened, writing...");
        await writer.WriteAsync($"GET {(isHttps ? "https" : "http")}://{_opts.Hostname}/ HTTP/1.1\r\n");
        await writer.WriteAsync($"Host: {_opts.Hostname}\r\n");
        await writer.WriteAsync($"User-Agent: {_opts.UserAgent}\r\n");
        await writer.WriteAsync($"\r\n");
        await writer.FlushAsync();
        _logger.LogTrace($"{address.Prefix(port)} Reading stream...");

        var line = await reader.ReadToEndAsync();
        var title = Regex.Match(line, "<title>(.+)</title>", RegexOptions.Multiline);
        if (title.Success)
        {
            _logger.LogInformation($"{address.Prefix(port)} Server responded with title: " + title.Groups[1].ToString());
        }
        else
        {
            _logger.LogInformation($"{address.Prefix(port)} Server responded with: " + line);
        }

        try
        {
            await stream.DisposeAsync();
        }
        catch{}
    }
}