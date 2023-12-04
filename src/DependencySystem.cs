using System;
using System.Reflection;
using System.Collections.Generic;

namespace Blindness;

using Exceptions;

public class DependencySystem
{
    private DependencySystem() { }
    private static DependencySystem crr = new();
    public static DependencySystem Current => crr;

    public static void Reset()
        => crr = new();
    
    private Dictionary<Type, Type> typeMap = new();

    public Type GetConcreteType(Type type)
    {
        try
        {
            var concreteType = findConcrete(type);
            return concreteType;
        }
        catch (MissingConcreteTypeException ex)
        {
            throw ex;
        }
    }

    public Node GetConcrete(Type type)
    {
        try
        {
            var concreteType = findConcrete(type);
            var obj = Activator.CreateInstance(concreteType);

            var node = obj as Node;
            if (node is null)
                return null;
            
            node.LoadDependencies();
            return node;
        }
        catch (MissingConcreteTypeException ex)
        {
            throw ex;
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
            bool finded = false;
            foreach (var inter in type.GetInterfaces())
            {
                if (inter == inputType)
                    finded = true;
            }
            if (!finded)
                continue;
            
            if (type.GetCustomAttribute<ConcreteAttribute>() is null)
                continue;
            
            this.typeMap.Add(inputType, type);
            return type;
        }

        throw new MissingConcreteTypeException(inputType);
    }
}