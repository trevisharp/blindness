/* Author:  Leonardo Trevisan Silio
 * Date:    16/07/2024
 */
using System;
using System.Reflection;
using System.Collections.Generic;

namespace Blindness.States;

using Internal;
using Exceptions;
using Concurrency;

/// <summary>
/// Dependency injection system.
/// </summary>
public class DependencySystem(IAsyncModel model)
{
    private static DependencySystem crr = null;
    public static DependencySystem Current => crr;

    /// <summary>
    /// Reset a Dependency system sending a model used to init all nodes.
    /// </summary>
    public static void Reset(IAsyncModel model)
        => crr = new(model);
    
    private Assembly crrAssembly = null;
    private Dictionary<Type, Type> typeMap = [];
    
    /// <summary>
    /// Update assembly type used to find concrete types.
    /// </summary>
    public void UpdateAssembly(Assembly assembly)
    {
        crrAssembly = assembly;
        typeMap = [];
    }

    /// <summary>
    /// Get a concrete object of a Node based on your type.
    /// </summary>
    public Node GetConcrete(Type type)
    {
        try
        {
            var concreteType = FindConcrete(type);
            var obj = Activator.CreateInstance(concreteType);

            if (obj is not Node node)
                return null;

            node.Model = model;
            node.LoadDependencies();
            node.OnLoad();

            return node;
        }
        catch (MissingConcreteTypeException)
        {
            throw;
        }
        catch (TargetInvocationException ex)
        {
            throw new ActivatorException(ex.InnerException, type);
        }
        catch (Exception ex)
        {
            throw new ActivatorException(ex, type);
        }
    }

    Type FindConcrete(Type inputType)
    {
        if (typeMap.TryGetValue(inputType, out Type type))
            return type;
        
        if (crrAssembly is not null)
            return FindAndMapType(inputType, crrAssembly);
        
        if (!inputType.IsAbstract && !inputType.IsInterface)
            return inputType;
        
        return FindAndMapType(inputType, inputType.Assembly);;
    }

    Type FindAndMapType(Type inputType, Assembly assembly)
    {
        var findedType = FindConcreteByAssembly(inputType, assembly);
        typeMap.Add(inputType, findedType);
        return findedType;
    }

    static Type FindConcreteByAssembly(Type inputType, Assembly assembly)
    {
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