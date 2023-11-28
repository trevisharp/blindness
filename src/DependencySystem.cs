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

    public T GetConcrete<T>()
        where T : Node<T>
    {
        try
        {
            var inputType = typeof(T);
            var obj = GetConcrete(inputType);
            return obj as T;
        }
        catch (Exception ex)
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
            
            node.LoadMembers();
            node.Load();
            return node;
        }
        catch (MissingConcreteTypeException ex)
        {
            throw ex;
        }
        catch (Exception ex)
        {
            throw new ActivatorException(ex);
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
            if (!type.IsSubclassOf(inputType))
                continue;
            
            if (type.GetCustomAttribute<ConcreteAttribute>() is null)
                continue;
            
            this.typeMap.Add(inputType, type);
            return type;
        }

        throw new MissingConcreteTypeException(inputType);
    }
}