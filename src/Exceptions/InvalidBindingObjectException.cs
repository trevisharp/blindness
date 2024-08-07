/* Author:  Leonardo Trevisan Silio
 * Date:    07/08/2024
 */
using System;

namespace Blindness.Exceptions;

/// <summary>
/// Represents a error that occurs during binding operation.
/// </summary>
public class InvalidBindingObjectException(object obj) : Exception
{
    public override string Message =>
        $"""
        The object of type '{obj.GetType()}' cannot be used in binding because has not a internal Binding.
        To solve this problem add to the type:
        Binding Bind;
        """;
}