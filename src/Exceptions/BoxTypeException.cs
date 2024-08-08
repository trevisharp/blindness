/* Author:  Leonardo Trevisan Silio
 * Date:    31/07/2024
 */
using System;

namespace Blindness.Exceptions;

using Bind.Boxes;

/// <summary>
/// Represents a error that occurs when a object that is not
/// a box is used like a box.
/// </summary>
public class BoxTypeException(object value, string boxType = null) : Exception
{
    /// <summary>
    /// Throws an BoxTypeException if value is not from Box<T> type.
    /// </summary>
    public static void ThrowIfIsNotABox(object value)
    {
        if (Box.IsBox(value))
            return;
        
        throw new BoxTypeException(value);
    }

    public override string Message =>
        $"""
        The value {value} is not a {boxType ?? "box"}.
        """;
}