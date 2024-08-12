/* Author:  Leonardo Trevisan Silio
 * Date:    12/08/2024
 */
using System;

namespace Blindness.Core.Exceptions;

public class MissingEmptyConstructor(Type type) : Exception
{
    public override string Message => 
        $"""
        The type {type} has no empty constructor to create node. 
        """;
}