using System.Globalization;
using System.Net;
using System.Reflection;
using System.Text;

namespace DomainKnock;

public static class Extensions
{
    public static string Prefix(this IPAddress address, ushort port) => $"[{address}:{port}] ~";

    public static uint ToReadableFormat(this IPAddress addr) => BitConverter.ToUInt32(addr.GetAddressBytes().Reverse().ToArray());
    public static byte[] ToUtf8(this string str) => Encoding.UTF8.GetBytes(str);
    public static Version GetVersion(this Assembly? assembly) => assembly?.GetName()?.Version ?? new Version(1, 0, 0);

    public static bool SplitIfContains(this string input, string pattern, out string[] result)
    {
        if (input.Contains(pattern))
        {
            result = input.Split(pattern);
            return true;
        }
        
        result = Array.Empty<string>();
        return false;
    }

    public static string[] SplitAlways(this string input, string pattern)
    {
        return input.SplitIfContains(pattern, out var result) ? result : new string[] { input };
    }

    public static string Exclude(this string input, params char[] chars) 
        => chars.Aggregate(input, (current, chr) 
            => current.Replace(chr.ToString(), ""));

    public static string Exclude(this string input, Func<char, bool> isChar)
        => string.Concat(input.ToCharArray().Where(x => !isChar(x)));
}

public static class ExitCodes
{
    public const int Ok = 0;
    public const int InvalidCommand = 0x10000000;
}