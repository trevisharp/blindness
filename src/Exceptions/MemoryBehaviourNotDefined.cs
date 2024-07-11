/* Author:  Leonardo Trevisan Silio
 * Date:    11/07/2024
 */
using System;

namespace Blindness.Exceptions;

/// <summary>
/// Represents a error that occurs when memory behavior is not defined.
/// </summary>
public class MemoryBehaviourNotDefined : Exception
{
    public override string Message => 
        $"""
        The IMemoryBehavior is not defined. Use Memory.Reset(yourMemoryBehavior) to define it.
        """;
}