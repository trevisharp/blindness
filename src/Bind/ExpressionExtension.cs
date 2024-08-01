/* Author:  Leonardo Trevisan Silio
 * Date:    31/07/2024
 */
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Blindness.Bind;

/// <summary>
/// A extension class to System.Linq.Expression types to
/// handle expressions more easely.
/// </summary>
public static class ExpressionExtension
{
    /// <summary>
    /// Try get a member data value with a specific name
    /// from a specific object.
    /// </summary>
    public static bool TryGetData(
        this object obj,
        string memberName,
        out object value
    )
    {
        var type = obj.GetType();

        var property = type.GetProperty(memberName);
        if (property is not null)
        {
            value = GetProperty(obj, property); 
            return true;
        }

        var field = type.GetField(memberName);
        if (field is not null)
        {
            value = GetField(obj, field);
            return true;
        }

        value = null;
        return false;
    }

    static object GetProperty(object obj, PropertyInfo prop)
    {
        if (obj is null || prop is null)
            return null;
        var getMethod = prop.GetGetMethod();
        return getMethod.Invoke(obj, []);
    }

    static object GetField(object obj, FieldInfo field)
    {
        if (obj is null || field is null)
            return null;
        return field.GetValue(obj);
    }

    // public static bool TryReadMember(
    //     this Expression expression, 
    //     object obj, out object value)
    // {
    //     value = null;
    //     if (expression is not MemberExpression member)
    //         return false;
        
        
    // }
}