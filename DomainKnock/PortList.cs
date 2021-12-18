using System.Collections;

namespace DomainKnock;

/// <summary>
/// Struct that parses ports from a string.
///
/// Example of inputs:
/// 80-8080,8085,9090-9099 : 80,81,82,83,...,8080,8085,9090,9091,9092,...,9099
/// </summary>
internal readonly struct PortList : IEquatable<PortList>, IEnumerable<ushort>
{
    /// <summary>
    /// Port list. It's used as a list because this is a struct, thus I must prevent
    /// any sort of NullRefEx when using the un-overrideable public, parameter-less constructor.
    /// </summary>
    private readonly List<ushort> _ports = new();

    /// <summary>
    /// Whether any valid ports have been added on the object's constructor.
    /// </summary>
    public bool IsValid { get; }

    /// <summary>
    /// Creates a list of ports based on the ports from <param name="ports"></param>
    /// </summary>
    /// <param name="ports">Ports to be added</param>
    public PortList(IEnumerable<ushort> ports)
    {
        _ports.AddRange(ports);
        IsValid = _ports.Any();
    }

    /// <summary>
    /// Accepts string as a value as means to input a port list argument.
    /// </summary>
    /// <param name="input"></param>
    public static implicit operator PortList(string input)
        => Parse(input);

    /// <summary>
    /// Makes an attempt to parse input using <see cref="input"/> but suppressing any <see cref="PortListException"/>.
    /// </summary>
    public static bool TryParse(string input, out PortList portList)
    {
        try
        {
            portList = Parse(input);
            return portList.IsValid;
        }
        catch (PortListException)
        {
            portList = new();
            return false;
        }
    }

    /// <summary>
    /// Parses <param name="input"></param> and returns a <see cref="PortList"/>,
    /// which can be enumerated to get a list of ports.
    /// </summary>
    /// <param name="input">Port list argument</param>
    /// <exception cref="PortListException">Thrown if the <param name="input"></param> is malformed.</exception>
    public static PortList Parse(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return new();

        // no spaces allowed
        input = input.Exclude(' ');

        // separate by ',' and store as tokens for later-processing
        List<string> tokens = new();
        if (input.SplitIfContains(",", out var tokenRange))
            tokens.AddRange(tokenRange);
        else
            tokens.Add(input);

        // no tokens found means there is probably no data. So we will not parse.
        if (tokens.Count == 0) return new();

        // we define the port list which will be used to instantiate the class later.
        // all parsed tokens will be added to this list.
        List<ushort> ports = new();

        // we loop through the parsed tokens and add the ports (permissive).
        foreach (var port in GetPortsByTokens(tokens))
            if (!ports.Contains(port))
                ports.Add(port);

        return new(ports);
    }

    /// <summary>
    /// Parses a list of tokens and gives a list of valid ports.
    /// </summary>
    /// <param name="tokens">An unparsed token list (e.g. INPUT: {"12","34","36-39"})</param>
    /// <returns>A parsed tokens list transformed into ports (e.g. OUTPUT: {12,34,36,37,38,39})</returns>
    /// <exception cref="PortListException">Thrown if there's any malformed token(s).</exception>
    private static IEnumerable<ushort> GetPortsByTokens(IEnumerable<string> tokens)
    {
        // we read each token into ports
        // we are being permissive here (allowing ports to be repeated). Might be changed in the future.
        // we allow a port to be any number between 0 and 65535.

        foreach (var token in tokens)
        {
            // if it's a range or not. A range is: portOrigin-portDestination (e.g. 80-8080 - all ips including and in between 80 and 8080).
            if (token.SplitIfContains("-", out var ranges))
            {
                // we only accept ranges of two numbers (of unsigned short)
                if (ranges.Length != 2)
                    throw new PortListException($"You can only have two numbers in the range. {ranges.Length} given", token);

                // we check the ports for two valid ushort values
                if (!ushort.TryParse(ranges[0], out var origin))
                    throw new PortListException($"Invalid port specified in range: {ranges[0]}. A port MUST BE a number between {ushort.MinValue}-{ushort.MaxValue}", token);
                if (!ushort.TryParse(ranges[1], out var destination))
                    throw new PortListException($"Invalid port specified in range: {ranges[1]}. A port MUST BE a number between {ushort.MinValue}-{ushort.MaxValue}", token);

                // swaps if destination is greater than origin.
                if (origin > destination)
                    (origin, destination) = (destination, origin);

                // all ports are added by doing a for-loop in origin-to-destination specified range (permissive)
                for (var portInRange = origin; portInRange <= destination; portInRange++)
                    yield return portInRange;
            }
            else
            {
                // if it's not a range, then it will only consider as a port.
                // we now validate 
                if (!ushort.TryParse(token, out var singlePort))
                    throw new PortListException($"The port specified is invalid", token);

                // a valid single port has been found.
                yield return singlePort;
            }
        }
    }

    /// <summary>
    /// This method is implicitly convertible to bool. This bool indicates whether it's valid or not.
    /// </summary>
    public static implicit operator bool(PortList portList)
        => portList.IsValid;

    /// <returns>All ports split by ','. No ranges.</returns>
    public override string ToString()
    {
        return string.Join(',', _ports);
    }
    public bool Equals(PortList other) => this && other && _ports.Equals(other._ports);
    public override bool Equals(object? obj) => obj is PortList other && Equals(other);
    public override int GetHashCode() => HashCode.Combine(nameof(PortList), _ports, IsValid);
    
    public IEnumerator<ushort> GetEnumerator() => (_ports ?? Enumerable.Empty<ushort>()).GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

/// <summary>
/// If there's any malformed token or input given to <see cref="PortList"/>
/// </summary>
public class PortListException : Exception
{
    public string? Token { get; }

    public PortListException(string message)
        : base(message) {}

    public PortListException(string message, string token)
        : this($"{message.TrimEnd('.')}. Parameter: {token}") => Token = token;
}