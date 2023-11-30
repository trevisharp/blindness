using System;

namespace Blindness.Exceptions;

public class ActivatorException : Exception
{
    Exception inner;
    Type type;
    public ActivatorException(Exception inner, Type type)
    {
        this.inner = inner;
        this.type = type;
    }

    public override string StackTrace =>
        $"{inner.StackTrace}\n{base.StackTrace}";

    public override string Message =>
        $"""
        The following error are throwed at instanciation of node '{type.Name}'.
        (Sometimes this error is caused by constructors in Node class)
            {inner.Message.Replace("\n", "\n\t")}
        """;
}