using System;

namespace Blindness.Exceptions;

public class InvalidBindingFormatException : Exception
{
    string extraInfo;
    public InvalidBindingFormatException(string extraInfo = null)
        => this.extraInfo = extraInfo;

    public override string Message =>
        $"A Binding function need be in a valid format. {extraInfo ?? ""}";
}