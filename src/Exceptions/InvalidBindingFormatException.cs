/* Author:  Leonardo Trevisan Silio
 * Date:    01/01/2024
 */
using System;

namespace Blindness.Exceptions;

/// <summary>
/// Represents a error that occurs during binding operation.
/// </summary>
public class InvalidBindingFormatException : Exception
{
    string extraInfo;
    public InvalidBindingFormatException(string extraInfo = null)
        => this.extraInfo = extraInfo;

    public override string Message =>
        $"A Binding function need be in a valid format. {extraInfo ?? ""}";
}