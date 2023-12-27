using System;
using System.Text;
using System.Security.Cryptography;

namespace Blindness.Internal;

internal static class HashExtensions
{
    public static string ToHash(this string text)
    {
        using var sha = SHA512.Create();
        var bytes = Encoding.Unicode.GetBytes(text);
        var hash = sha.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
}