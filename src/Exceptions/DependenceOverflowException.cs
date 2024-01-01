/* Author:  Leonardo Trevisan Silio
 * Date:    01/01/2024
 */
using System;

namespace Blindness.Exceptions;

/// <summary>
/// Represents a error that occurs when exist a dependence cycle in node definitions.
/// </summary>
public class DependenceOverflowException : Exception
{
    Type type;
    public DependenceOverflowException(Type type)
        => this.type = type;
    public override string Message => 
        $"""
        A cycle-dependece of nodes are detected in type {type}
        """;
}