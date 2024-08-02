/* Author:  Leonardo Trevisan Silio
 * Date:    31/07/2024
 */
using System;

namespace Blindness.Exceptions;

/// <summary>
/// Represents a error that occurs when a property to bind do not exist.
/// </summary>
public class MissingPropertyBindException(string prop, Type parentType) : Exception
{
    public override string Message =>
        $"""
        Missing a property/field {prop} on {parentType} to perform a Bind operation. Or the property/field is a method.
        """;
}