/* Author:  Leonardo Trevisan Silio
 * Date:    02/08/2024
 */
using System;

namespace Blindness.Exceptions;

/// <summary>
/// Represents a error that occurs when the bind syntax is in a incorret format.
/// </summary>
public class WrongFormatBindException(string expected, string current) : Exception
{
    public override string Message => 
        $"""
        The Bind syntax are be incorrect. The expected syntax are {expected}. But the current format as {current}.
        """;
}