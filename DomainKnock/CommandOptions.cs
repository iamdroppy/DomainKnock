using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using CommandLine.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DomainKnock;

internal class CommandOptions
{
    [Option('v', "verbose", HelpText = "Verbose Level (0 = Info, 1 = Debug, 2 = Trace)")]
    public int Verbose { get; set; }

    [Option('h', "hostname", Required = true)]
    public string Hostname { get; set; }

    [Option('o', "origin", HelpText = "Start IP address.", Required = true)]
    public string Origin { get; set; }
    [Option('d', "destination", HelpText = "End IP address.", Required = true)]
    public string Destination { get; set; }

    [Option('a', "user-agent", HelpText = "User-Agent ('DomainKnock' default)")]
    public string UserAgent { get; set; } = "DomainKnock";

    [Option('t', "timeout", HelpText = "Timeout in seconds (15 default)")]
    public int TimeoutSeconds { get; set; } = 15;

    [Option('p', "progress-delay",
        HelpText = "Delay (in seconds) between each progress scanning message. (30 default)")]
    public int ProgressDelay { get; set; } = 30;

    public static CommandOptions Parse(string[] args)
    {
        CommandOptions opts = null;
        Parser.Default
            .ParseArguments<CommandOptions>(args)
            .WithParsed(o => opts = o);
        if (opts == null) Environment.Exit(1);
        return opts;
    }
}
