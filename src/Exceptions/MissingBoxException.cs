/* Author:  Leonardo Trevisan Silio
 * Date:    30/07/2024
 */
using System;

namespace Blindness.Exceptions;

/// <summary>
/// Represents a error that occurs when a box do not exists in a BoxDictionary.
/// </summary>
public class MissingBoxException(string name) : Exception
{
    public override string Message =>
        $"""
        A box with name {name} do not exits. Use GetOrCreate, Open
        or Place method to create and initialize the box.
        """;
}