using System;

namespace Blindness.Exceptions;

public class InvalidBindingFormatException : Exception
{
    public override string Message =>
        """
        A Binding function need be in the format x => y.
        """;
}