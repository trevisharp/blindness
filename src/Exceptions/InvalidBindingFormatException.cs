/* Author:  Leonardo Trevisan Silio
 * Date:    11/07/2024
 */
using System;

namespace Blindness.Exceptions;

/// <summary>
/// Represents a error that occurs during binding operation.
/// </summary>
public class InvalidBindingFormatException(string extraInfo = null) : Exception
{
    public override string Message =>
        $"A Binding function need be in a valid format. {extraInfo ?? ""}";
}