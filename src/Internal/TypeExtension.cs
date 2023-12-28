using System;
using System.Linq;
using System.Reflection;

namespace Blindness.Internal;

public static class TypeExtension
{
    public static bool Implements(this Type type, Type baseType)
    {
        if (type is null)
            return false;
        
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
        if (type is null)
            return false;
        
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
}