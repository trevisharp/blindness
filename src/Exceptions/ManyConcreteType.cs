/* Author:  Leonardo Trevisan Silio
 * Date:    23/07/2024
 */
using System;

namespace Blindness.Exceptions;

/// <summary>
/// Represents a error that occurs when do many concrete types for a specific type.
/// </summary>
public class ManyConcreteTypeException(Type baseType) : Exception
{
    public override string Message =>
        $"""
        Cannot choose one of many subtypes of {baseType} type.
        """;
}