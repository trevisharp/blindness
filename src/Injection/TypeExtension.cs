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
    /// <summary>
    /// Return if a type implements a baseType.
    /// </summary>
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
    
    /// <summary>
    /// Return if a type implements a baseType.
    /// </summary>
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

    /// <summary>
    /// Find a Concrete type that implements inputType and has specified attribute
    /// in a specified assembly.
    /// </summary>
    public static Type FindConcreteByAssembly(this Type inputType, 
        Type attributeType, Assembly assembly)
    {
        ArgumentNullException.ThrowIfNull(inputType, nameof(inputType));
        ArgumentNullException.ThrowIfNull(assembly, nameof(assembly));

        var types = assembly.GetTypes();
        foreach (var type in types)
        {
            if (!type.Implements(inputType.Name))
                continue;
            
            if (type.GetCustomAttribute(attributeType) is null)
                continue;
            
            return type;
        }

        throw new MissingConcreteTypeException(inputType);
    }
    
    /// <summary>
    /// Find a Concrete type that implements inputType and has specified attribute
    /// in a specified assembly.
    /// </summary>
    public static Type FindByAssembly(this Type inputType, Assembly assembly)
    {
        ArgumentNullException.ThrowIfNull(inputType, nameof(inputType));
        ArgumentNullException.ThrowIfNull(assembly, nameof(assembly));

        var types = assembly.GetTypes();
        foreach (var type in types)
        {
            if (!type.Implements(inputType.Name))
                continue;
            
            return type;
        }

        throw new MissingConcreteTypeException(inputType);
    }
}