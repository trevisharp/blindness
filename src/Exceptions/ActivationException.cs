using System;

namespace Blindness.Exceptions;

public class ActivatorException : Exception
{
    Exception inner;
    public ActivatorException(Exception inner)
        => this.inner = inner;

    public override string Message =>
        $"""
        The follow error are throwed at instanciation of concrete node:
            {inner.Message}
        Sometimes this error is caused by constructors in Node class.
        """;
}