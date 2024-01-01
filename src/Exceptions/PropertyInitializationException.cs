/* Author:  Leonardo Trevisan Silio
 * Date:    01/01/2024
 */
using System;

namespace Blindness.Exceptions;

/// <summary>
/// Represents a error that occurs on a property initialization.
/// </summary>
public class PropertyInitializationException : Exception
{
    private string property;
    private Exception inner;
    public PropertyInitializationException(string property, Exception inner)
    {
        this.property = property;
        this.inner = inner;
    }

    public override string StackTrace =>
        $"{inner.StackTrace}\n{base.StackTrace}";
    
    public override string Message =>
        $"""
        The following error has throwed on property '{property}' initialization:
            {inner.Message.Replace("\n", "\n\t")}
        """;
}