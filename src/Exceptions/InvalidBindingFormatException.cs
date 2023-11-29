using System;

namespace Blindness.Exceptions;

public class InvalidBindingFormatException : Exception
{
    string extraInfo;
    public InvalidBindingFormatException(string extraInfo = null)
        => this.extraInfo = extraInfo;

    public override string Message =>
        $"A Binding function need be in the format x => y. {extraInfo ?? ""}";
}