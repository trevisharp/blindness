using System;

namespace Blindness.Exceptions;

public class MissingConcreteTypeException : Exception
{
    Type baseType;
    public MissingConcreteTypeException(Type baseType)
        => this.baseType = baseType;

    public override string Message =>
        $"""
        Missing a subtype of {baseType} with concrete attribute. 
        """;
}