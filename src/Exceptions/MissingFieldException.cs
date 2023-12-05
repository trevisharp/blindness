using System;

namespace Blindness.Exceptions;

public class MissingFieldException : Exception
{
    string field;
    Type parentType;
    public MissingFieldException(string field, Type parentType)
    {
        this.field = field;
        this.parentType = parentType;
    }

    public override string Message =>
        $"""
        The type '{parentType}' does not have a field '{field}'.
        """;
}