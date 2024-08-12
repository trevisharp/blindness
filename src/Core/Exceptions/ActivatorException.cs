/* Author:  Leonardo Trevisan Silio
 * Date:    12/08/2024
 */
using System;

namespace Blindness.Core.Exceptions;

/// <summary>
/// Represents error that occurs during instantiation of a Type.
/// </summary>
public class ActivatorException(Exception inner, Type type) : Exception
{
    public override string StackTrace =>
        $"{inner.StackTrace}\n{base.StackTrace}";

    public override string Message =>
        $"""
        The following error are throwed on constructor or Deps method invokation from type '{type}':
            {inner.Message.Replace("\n", "\n\t")}
        """;
}