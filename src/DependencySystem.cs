using System;
using System.Reflection;
using System.Collections.Generic;

namespace Blindness;

public class DependencySystem
{
    private DependencySystem() { }
    private static DependencySystem crr = new();
    public static DependencySystem Current => crr;

    public static void Reset()
        => crr = new();
    
    private Dictionary<Type, Type> typeMap = new();

    public T GetConcrete<T>()
        where T : Node<T>
    {
        var inputType = typeof(T);
        
        if (typeMap.ContainsKey(inputType))
            return null;

        var assembly = Assembly.GetEntryAssembly();
        var types = assembly.GetTypes();

        foreach (var type in types)
        {
            if (!type.IsSubclassOf(inputType))
                continue;
            
            
        }

        throw new NotImplementedException();
    }
}