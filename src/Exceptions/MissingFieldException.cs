/* Author:  Leonardo Trevisan Silio
 * Date:    01/01/2024
 */
using System;

namespace Blindness.Exceptions;

/// <summary>
/// Represents a error that occurs when a type do not have a specific field.
/// </summary>
public class MissingFieldException : Exception
{
    string field;
    Type parentType;
    public MissingFieldException(string field, Type parentType)
    {
        this.field = field;
        this.parentType = parentType;
    }

    public override string Message =>
        $"""
        The type '{parentType}' does not have a field '{field}'.
        """;
}