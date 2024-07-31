/* Author:  Leonardo Trevisan Silio
 * Date:    30/07/2024
 */
using System;

namespace Blindness.Exceptions;

/// <summary>
/// Represents a error that occurs when a object in BoxDictionary is
/// not a box.
/// </summary>
public class BoxTypeException(string name) : Exception
{
    public override string Message =>
        $"""
        The value with name {name} are not a box. Use Place
        to BoxDictionary or using Binding.Place with only Box<T>
        values.
        """;
}