/* Author:  Leonardo Trevisan Silio
 * Date:    05/08/2024
 */
using System;
using System.Reflection;

namespace Blindness.Exceptions;

/// <summary>
/// Represents a error that occurs when a operation maked with a field/property
/// like get or set that not exists.
/// </summary>
public class InvalidMemberOperationException(object obj, MemberInfo member, string function) : Exception
{
    public override string Message => 
        $"""
        The member {member.Name} in type {obj.GetType()} not support {function} operation.
        Consider find a bind that try {function} this value and remove the bind to avoid this error.
        """;
}