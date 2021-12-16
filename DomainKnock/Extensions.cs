using System.Net;
using System.Text;

namespace DomainKnock;

public static class Extensions
{
    public static uint ToReadableFormat(this IPAddress addr) => BitConverter.ToUInt32(addr.GetAddressBytes().Reverse().ToArray());
    public static byte[] ToUtf8(this string str) => Encoding.UTF8.GetBytes(str);
}