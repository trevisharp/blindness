/* Author:  Leonardo Trevisan Silio
 * Date:    15/07/2024
 */
using System;
using System.Text;
using System.Security.Cryptography;

namespace Blindness.Internal;

internal static class HashExtensions
{
    /// <summary>
    /// Get Sha256 base64 encoded hash from a string.
    /// </summary>
    public static string ToHash(this string text)
    {
        var bytes = Encoding.Unicode.GetBytes(text);
        var hash = SHA512.HashData(bytes);
        return Convert.ToBase64String(hash);
    }
}