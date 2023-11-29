using System;

namespace Blindness.Exceptions;

public class PropertyInitializationException : Exception
{
    private string property;
    private Exception inner;
    public PropertyInitializationException(string property, Exception inner)
    {
        this.property = property;
        this.inner = inner;
    }
    
    public override string Message =>
        $"""
        The following error has throwed on property {property} initialization:
            {inner.Message}
        """;
}