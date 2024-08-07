/* Author:  Leonardo Trevisan Silio
 * Date:    07/08/2024
 */
using System;

namespace Blindness.Exceptions;

/// <summary>
/// Represents a error that occurs when a bind between two readonly data is called.
/// </summary>
public class ReadonlyBindingException : Exception
{
    public override string Message =>
        $"""
        A binding between two readonly data is invalid.
        """;
}