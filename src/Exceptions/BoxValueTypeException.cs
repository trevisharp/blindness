/* Author:  Leonardo Trevisan Silio
 * Date:    02/08/2024
 */
using System;

namespace Blindness.Exceptions;

using Bind;

/// <summary>
/// Represents a error that occurs when a object that is not
/// a box is used like a box.
/// </summary>
public class BoxValueTypeException(object value, Type type) : Exception
{
    /// <summary>
    /// Throws an BoxTypeException if value is not from Box<T> type.
    /// </summary>
    public static void ThrowIfIsIncorrectType(object value, Type type)
    {
        if (Box.IsBoxType(value, type))
            return;
        
        throw new BoxValueTypeException(value, type);
    }

    public override string Message =>
        $"""
        The box {value} needed has a type {type}.
        """;
}