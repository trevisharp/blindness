/* Author:  Leonardo Trevisan Silio
 * Date:    11/07/2024
 */
using System;

namespace Blindness.Exceptions;

/// <summary>
/// Represents a error that occurs when a type do not have a specific field.
/// </summary>
public class MissingFieldException(string field, Type parentType) : Exception
{
    public override string Message =>
        $"""
        The type '{parentType}' does not have a field '{field}'.
        """;
}