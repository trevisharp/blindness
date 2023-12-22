using System;

namespace Blindness.Exceptions;

public class MemoryBehaviourNotDefined : Exception
{
    public override string Message => 
        $"""
        The IMemoryBehavior is not defined. Use Memory.Reset(yourMemoryBehavior) to define it.
        """;
}