/* Author:  Leonardo Trevisan Silio
 * Date:    18/07/2024
 */
using System;
using System.Reflection;
using System.Collections.Generic;

namespace Blindness.Injection;

using Exceptions;

/// <summary>
/// Dependency injection system.
/// </summary>
public class DependencySystem
{
    private static DependencySystem shared = null;

    /// <summary>
    /// Get a global DependencySystem reference.
    /// </summary>
    public static DependencySystem Shared => shared;

    /// <summary>
    /// Reset a Dependency system sending a model used to init all nodes.
    /// </summary>
    public static void Reset()
        => shared = new();
    
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
    public T GetConcrete<T>(Type type)
    {
        try
        {
            var concreteType = FindConcrete(type);
            var obj = Activator.CreateInstance(concreteType);
            return (T)obj;
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
        
        return FindAndMapType(inputType, inputType.Assembly);
    }

    Type FindAndMapType(Type inputType, Assembly assembly)
    {
        var findedType = inputType.FindConcreteByAssembly(assembly);
        typeMap.Add(inputType, findedType);
        return findedType;
    }
}