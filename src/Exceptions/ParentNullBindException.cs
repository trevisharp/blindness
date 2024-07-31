/* Author:  Leonardo Trevisan Silio
 * Date:    31/07/2024
 */
using System;

namespace Blindness.Exceptions;

/// <summary>
/// Represents a error that occurs when the parent of Binding type is null.
/// </summary>
public class ParentNullBindException : Exception
{
    public override string Message =>
        $"""
        The parent of Bind type is null, check the Binding object initialization to fix this problem.
        """;
}