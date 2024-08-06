/* Author:  Leonardo Trevisan Silio
 * Date:    06/08/2024
 */
using System;

namespace Blindness.Exceptions;

/// <summary>
/// Represents a error that occurs when a readonly box's Place method are called.  
/// </summary>
public class ReadonlyBoxException : Exception
{
    public override string Message =>
        $"""
        A readonly box cannot be placed.
        This error may occurs when a Readonly binding are performed and the value are changed. Like:
            - A binding on a only-get property.
            - A binding on a method result.
            - A binding on a constant.
            - A binding on a binary operation.
        """;
}