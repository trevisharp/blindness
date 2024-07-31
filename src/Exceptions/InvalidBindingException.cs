/* Author:  Leonardo Trevisan Silio
 * Date:    31/07/2024
 */
using System;

namespace Blindness.Exceptions;

/// <summary>
/// Represents a error that occurs when exist a dependence cycle in node definitions.
/// </summary>
public class InvalidBindingException(string extraMessage = "") : Exception
{
    public override string Message => 
        $"""
        The binding sintax applied it is invalid and cannot be handled. {extraMessage}
        Consider add a new BindChainLink, create a new BindBehaviour to extend the bind operation.
        """;
}