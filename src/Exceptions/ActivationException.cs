/* Author:  Leonardo Trevisan Silio
 * Date:    01/01/2024
 */
using System;

namespace Blindness.Exceptions;

/// <summary>
/// Represents error that occurs during instantiation of a Node.
/// </summary>
public class ActivatorException(Exception inner, Type type) : Exception
{
    public override string StackTrace =>
        $"{inner.StackTrace}\n{base.StackTrace}";

    public override string Message =>
        $"""
        The following error are throwed at creation of node '{type.Name}'.
            {inner.Message.Replace("\n", "\n\t")}
        """;
}