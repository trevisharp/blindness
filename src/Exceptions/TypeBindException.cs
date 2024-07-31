/* Author:  Leonardo Trevisan Silio
 * Date:    31/07/2024
 */
using System;

namespace Blindness.Exceptions;

/// <summary>
/// Represents a error that occurs when a bind has a incorrect type.
/// </summary>
public class TypeBindException(object type, string key, Type expected, Type bindType) : Exception
{
    public override string Message =>
        $"""
        A bind with field {key} in the parent type {type} needed recive a {expected} type, but the expression returns a {bindType} type.
        """;
}