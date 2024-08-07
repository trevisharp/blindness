/* Author:  Leonardo Trevisan Silio
 * Date:    23/07/2024
 */
using System;

namespace Blindness.Exceptions;

/// <summary>
/// Represents a error that occurs when do not exist a concrete type for a specific type.
/// </summary>
public class MissingConcreteTypeException(Type baseType) : Exception
{
    public override string Message =>
        $"""
        Missing a subtype of {baseType}. 
        """;
}