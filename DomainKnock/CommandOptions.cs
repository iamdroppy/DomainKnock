using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CommandLine;
using CommandLine.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DomainKnock;

internal class CommandOptions
{
    [Option('v', "verbose", HelpText = "Verbosity Level (0 = Info, 1 = Debug, 2 = Trace)")]
    public int Verbose { get; set; }

    [Option('h', "hostname", HelpText = "The hostname the IP(s) should respond to.", Required = true)]
    public string Hostname { get; set; }

    [Option("from", HelpText = "Start IP address. Use only this argument if you want to check a single IP Address", Required = true)]
    public string Origin { get; set; }

    [Option("to", HelpText = "End IP address. Use only --from if you want to check a single IP Address")]
    public string Destination { get; set; }

    [Option("http-ports", HelpText = "Http Ports (e.g. '80,8080-8099,9090-9100')", Group = "Ports")]
    public string HttpPorts { get; set; } = "";

    [Option("https-ports", HelpText = "Https (HTTP over SSL) Ports (e.g. '443,8443-8449')", Group = "Ports")]
    public string HttpsPorts { get; set; } = "";

    [Option('t', "timeout", HelpText = "Timeout (in seconds) of each TCP connection and protocol negotiation (Default: 5)")]
    public int TimeoutSeconds { get; set; } = 5;

    [Option('d', "progress-delay",
        HelpText = "Delay (in seconds) between each progress scanning message. 0 to disable. (Default: 30)")]
    public uint ProgressDelay { get; set; } = 30;

    [Option("agent", HelpText = "User-Agent passed via Headers (Default: 'DomainKnock')")]
    public string UserAgent { get; set; } = "DomainKnock";

    public static CommandOptions? Parse(string[] args)
    {
        CommandOptions opts = null;
        Parser.Default
            .ParseArguments<CommandOptions>(args)
            .WithParsed(o => opts = o);
        if (opts is not null && string.IsNullOrWhiteSpace(opts.Destination))
            opts.Destination = opts.Origin;
        return opts;
    }
}
