/* Author:  Leonardo Trevisan Silio
 * Date:    18/07/2024
 */
using System;
using System.Linq;
using System.Reflection;

namespace Blindness.Injection;

using Exceptions;

/// <summary>
/// A class contains many useful methods for Type objects.
/// </summary>
public static class TypeExtension
{
    public static bool Implements(this Type type, Type baseType)
    {
        ArgumentNullException.ThrowIfNull(type, nameof(type));
        ArgumentNullException.ThrowIfNull(baseType, nameof(baseType));
        
        if (type == baseType)
            return true;

        if (type.BaseType?.Implements(baseType) ?? false) 
            return true;
        
        if (!baseType.IsInterface)
            return false;

        var interfaces = type.GetInterfaces();
        if (interfaces.Contains(baseType))
            return true;

        foreach (var baseInterface in interfaces)
            if (baseInterface.Implements(baseType))
                return true;

        return false;
    }
    
    public static bool Implements(this Type type, string baseType)
    {
        ArgumentNullException.ThrowIfNull(type, nameof(type));
        ArgumentNullException.ThrowIfNull(baseType, nameof(baseType));
        
        var nonGeneratedName = type.Name
            .Replace("<>c", "")
            .Replace("+", "");
        if (nonGeneratedName == baseType)
            return true;

        if (type.BaseType?.Implements(baseType) ?? false) 
            return true;

        var interfaces = type.GetInterfaces();
        var names = interfaces
            .Select(i => i.Name);
        if (names.Contains(baseType))
            return true;

        foreach (var baseInterface in interfaces)
            if (baseInterface.Implements(baseType))
                return true;

        return false;
    }

    public static Type FindConcreteByAssembly(this Type inputType, Assembly assembly)
    {
        ArgumentNullException.ThrowIfNull(inputType, nameof(inputType));
        ArgumentNullException.ThrowIfNull(assembly, nameof(assembly));

        var types = assembly.GetTypes();
        foreach (var type in types)
        {
            if (!type.Implements(inputType.Name))
                continue;
            
            if (type.GetCustomAttribute<ConcreteAttribute>() is null)
                continue;
            
            return type;
        }

        throw new MissingConcreteTypeException(inputType);
    }
}