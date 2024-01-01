/* Author:  Leonardo Trevisan Silio
 * Date:    01/01/2024
 */
using System;

namespace Blindness.Exceptions;

/// <summary>
/// Represents a error that occurs when try to run a NonAsyncElement node.
/// </summary>
public class NonAsyncElementException : Exception
{
    Type type;
    public NonAsyncElementException(Type type)
        => this.type = type;
    
    public override string Message => 
        $"""
        The type {type} do not is a IAsyncElement to run in this function.
        Consider use a type that inherits Node class.
        """;
}