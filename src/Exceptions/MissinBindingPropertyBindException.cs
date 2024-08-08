/* Author:  Leonardo Trevisan Silio
 * Date:    08/08/2024
 */
using System;

namespace Blindness.Exceptions;

/// <summary>
/// Represents a error that occurs when the left is not Binding Property.
/// </summary>
public class MissinBindingPropertyBindException : Exception
{
    public override string Message => 
        $$"""
        The left side of a Bind expression need be a Binding expression like this:
        [Binding]
        public int Value
        {
            get => Get(this).Open<int>(nameof(Value));
            set => Get(this).Place(nameof(Value), value);
        }
        """;
}