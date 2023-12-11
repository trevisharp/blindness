using System;
using System.Reflection;
using System.Collections.Generic;

namespace Blindness.Internal;

using Exceptions;

internal class DependencySystem
{
    private DependencySystem() { }
    private static DependencySystem crr = new();
    public static DependencySystem Current => crr;

    internal static void Reset()
        => crr = new();
    
    private Dictionary<Type, Type> typeMap = new();

    internal Type GetConcreteType(Type type)
    {
        var concreteType = findConcrete(type);
        return concreteType;
    }

    internal Node GetConcrete(Type type)
    {
        try
        {
            var concreteType = findConcrete(type);
            var obj = Activator.CreateInstance(concreteType);

            var node = obj as Node;
            if (node is null)
                return null;
            
            node.LoadDependencies();
            node.OnLoad();

            return node;
        }
        catch (MissingConcreteTypeException ex)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ActivatorException(ex, type);
        }
    }

    private Type findConcrete(Type inputType)
    {
        if (typeMap.ContainsKey(inputType))
            return typeMap[inputType];

        var assembly = inputType.Assembly;
        var types = assembly.GetTypes();

        foreach (var type in types)
        {
            if (!implementsInterface(type, inputType))
                continue;
            
            if (type.GetCustomAttribute<ConcreteAttribute>() is null)
                continue;
            
            this.typeMap.Add(inputType, type);
            return type;
        }

        throw new MissingConcreteTypeException(inputType);
    }

    private bool implementsInterface(Type type, Type interfaceType)
    {
        foreach (var inter in type.GetInterfaces())
            if (inter == interfaceType)
                return true;
        
        return false;
    }
}