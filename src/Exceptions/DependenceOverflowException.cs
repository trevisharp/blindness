/* Author:  Leonardo Trevisan Silio
 * Date:    11/07/2024
 */
using System;

namespace Blindness.Exceptions;

/// <summary>
/// Represents a error that occurs when exist a dependence cycle in node definitions.
/// </summary>
public class DependenceOverflowException(Type type) : Exception
{
    public override string Message => 
        $"""
        A cycle-dependece of nodes are detected in type {type}
        """;
}