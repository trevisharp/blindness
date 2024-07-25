/* Author:  Leonardo Trevisan Silio
 * Date:    25/07/2024
 */
using System;

namespace Blindness.Exceptions;

/// <summary>
/// Represents error that occurs during a dependency loading.
/// </summary>
public class DependencyLoadingException(Exception inner, Type type) : Exception
{
    public override string StackTrace =>
        $"{inner.StackTrace}\n{base.StackTrace}";

    public override string Message =>
        $"""
        The following error are throwed at creation of type '{type}'.
            {inner.Message.Replace("\n", "\n\t")}
        """;
}