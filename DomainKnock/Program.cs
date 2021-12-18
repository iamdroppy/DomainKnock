using System.Reflection;
using DomainKnock;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

void PrintBanner()
{
    Console.ForegroundColor = ConsoleColor.DarkRed;
    Console.WriteLine(@"_ _             ");
    Console.WriteLine(@"| |__ _ _   ___  __ | |__ ___  _ _ ");
    Console.Write(@"| / /| ' \ / _ \/ _|| / // -_)| '_|");
    Console.ForegroundColor = ConsoleColor.White;
    Console.WriteLine("   Hostname brute-force for IP ranges - v" + Assembly.GetEntryAssembly().GetVersion());
    Console.ForegroundColor = ConsoleColor.DarkRed;
    Console.WriteLine(@"|_\_\|_||_|\___/\__||_\_\\___||_|  ");
    Console.ResetColor();
    Console.WriteLine();
}

PrintBanner();

async Task RunAsync(params string[] arguments)
{
    try
    {
        var options = CommandOptions.Parse(arguments);
        if (options == null) return;

        var logLevel = options.Verbose switch
        {
            <= 0 => LogLevel.Information,
            1 => LogLevel.Debug,
            >= 2 => LogLevel.Trace,
        };

        ServiceCollection services = new();

        services
            .AddSingleton(options)
            .AddTransient<Knocker>()
            .AddTransient<InitialTcpConnection>()
            .AddTransient<HttpHandler>()
            .AddLogging(s => s.AddConsole().SetMinimumLevel(logLevel));

        await using var provider = services.BuildServiceProvider();

        var knocker = provider.GetRequiredService<Knocker>();
        await knocker.StartAsync();
    }
    catch (ArgumentException ex)
    {
        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.WriteLine(ex.Message);
        Console.ResetColor();
    }
    catch (Exception ex)
    {
        Console.ForegroundColor = ConsoleColor.DarkRed;
        Console.Write("Unhandled error: ");
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine(ex.Message);
        Console.ResetColor();
    }
}

bool UseCli()
{
#if CLI
    return true;
#else
    if (args.Any(s => s == "--cli"))
    {
        Console.WriteLine("CLI argument has been provided - all the other arguments has been ignored.");
        return true;
    }

    var useCliEnv = Environment.GetEnvironmentVariable("USE_CLI");
    if (useCliEnv == null)
        return false;
    if (useCliEnv.Equals("true", StringComparison.CurrentCultureIgnoreCase))
        return true;
    if (Int32.TryParse(useCliEnv, out var res) && res == 1)
        return true;
    return false;
#endif
}

if (UseCli())
{
    while (true)
    {
        Console.Write("> ");
        var input = Console.ReadLine() ?? "";

        if (input == "clear" || input == "cls")
        {
            Console.Clear();
            PrintBanner();
        }
        else
            await RunAsync(input.SplitOrArray(" "));
    }
}
else
{
    await RunAsync(args);
}