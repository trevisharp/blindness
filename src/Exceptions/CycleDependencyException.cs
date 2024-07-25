/* Author:  Leonardo Trevisan Silio
 * Date:    25/07/2024
 */
using System;

namespace Blindness.Exceptions;

/// <summary>
/// Represents a error that occurs when a cycle in dependecy graph is finded.
/// </summary>
public class CycleDependencyException(Type type) : Exception
{
    public override string Message =>
        $"""
        A dependency cycle is finded to type {type}.
        """;
}