using System;

namespace Blindness.Exceptions;

public class DependenceOverflowException : Exception
{
    Type type;
    public DependenceOverflowException(Type type)
        => this.type = type;
    public override string Message => 
        $"""
            A cycle-dependece of nodes are detected in type {type}
        """;
}