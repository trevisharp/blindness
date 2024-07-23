/* Author:  Leonardo Trevisan Silio
 * Date:    23/07/2024
 */
using System;

namespace Blindness.Exceptions;

/// <summary>
/// Represents a error that occurs when do many constructors for a specific type.
/// </summary>
public class ManyConstructorsException(Type type) : Exception
{
    public override string Message =>
        $"""
        The type {type} has many avaliable constructors..
        """;
}