/* Author:  Leonardo Trevisan Silio
 * Date:    11/07/2024
 */
using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace Blindness.Abstracts;

/// <summary>
/// Represents a logic to generate code.
/// </summary>
public abstract class Implementation
{
    public abstract void ImplementType(
        ClassBuilder builder,
        string fileName, Type baseInterface,
        List<PropertyInfo> properties,
        List<MethodInfo> methods
    );
    
    protected static string ArrangeGenericTypeName(Type type)
    {
        var genericParams = type.GetGenericArguments();
        if (genericParams.Length == 0)
            return type.Name;

        var name = type.GetGenericTypeDefinition().Name;

        return type.Name.Replace("`1", "") 
            + "<" + string.Join(",",
                genericParams.Select(p => p.Name)
            ) + ">";
    }
}