using DomainKnock;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

ServiceCollection services = new();

Console.ForegroundColor = ConsoleColor.DarkRed;
Console.WriteLine(@"_ _             ");
Console.WriteLine(@"| |__ _ _   ___  __ | |__ ___  _ _ ");
Console.Write(@"| / /| ' \ / _ \/ _|| / // -_)| '_|");
Console.ForegroundColor = ConsoleColor.White;
Console.WriteLine("   Hostname brute-force for IP ranges");
Console.ForegroundColor = ConsoleColor.DarkRed;
Console.WriteLine(@"|_\_\|_||_|\___/\__||_\_\\___||_|  ");
Console.ResetColor();
Console.WriteLine();
var options = CommandOptions.Parse(args);

var logLevel = options.Verbose switch
{
    <= 0 => LogLevel.Information,
    1 => LogLevel.Debug,
    >= 2 => LogLevel.Trace,
};

services
    .AddSingleton(options)
    .AddTransient<Knocker>()
    .AddTransient<InitialTcpConnection>()
    .AddTransient<HttpHandler>()
    .AddLogging(s => s.AddConsole().SetMinimumLevel(logLevel));

var provider = services.BuildServiceProvider();

var knocker = provider.GetRequiredService<Knocker>();
try
{
    await knocker.StartAsync();
}
catch (Exception ex)
{
    var mainLogger = provider.GetRequiredService<ILogger<Program>>();
    mainLogger.LogCritical(ex.Message, ex);
}