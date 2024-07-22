/* Author:  Leonardo Trevisan Silio
 * Date:    11/07/2024
 */
using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace Blindness.Factory;

/// <summary>
/// Represents a logic to generate code for implement a base type.
/// </summary>
public abstract class BaseTypeImplementation
{
    public abstract void ImplementType(
        ClassBuilder builder,
        string fileName, 
        Type implementedType,
        List<PropertyInfo> properties,
        List<MethodInfo> methods
    );
    
    protected static string ArrangeGenericTypeName(Type type)
    {
        var genericParams = type.GetGenericArguments();
        if (genericParams.Length == 0)
            return type.Name;

        var name = type.GetGenericTypeDefinition().Name;

        // TODO: Update to arrange types with more than 1
        // generic parameter
        return type.Name.Replace("`1", "") 
            + "<" + string.Join(",",
                genericParams.Select(p => p.Name)
            ) + ">";
    }
}