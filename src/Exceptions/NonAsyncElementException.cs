using System;

namespace Blindness.Exceptions;

public class NonAsyncElementException : Exception
{
    Type type;
    public NonAsyncElementException(Type type)
        => this.type = type;
    
    public override string Message => 
        $"""
        The type {type} do not is a IAsyncElement to run in this function.
        Consider use a type that inherits Node class.
        """;
}