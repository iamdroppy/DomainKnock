using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.Logging;

namespace DomainKnock;

/// <summary>
/// Part 2: creates the initial TCP connection
/// </summary>
internal class InitialTcpConnection
{
    private readonly ILogger<InitialTcpConnection> _logger;
    private readonly CommandOptions _opts;

    public InitialTcpConnection(ILogger<InitialTcpConnection> logger, CommandOptions opts)
    {
        _logger = logger;
        _opts = opts;
    }

    public async Task<TcpClient> ConnectAsync(IPAddress ip, ushort port, CancellationToken token = default)
    {
        TcpClient client = new();
        await client.ConnectAsync(ip, port, token);
        _logger.LogDebug($"Tcp connected ({ip.ToString()}:{port})");
        return client;
    }
}