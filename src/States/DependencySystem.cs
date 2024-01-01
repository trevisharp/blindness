/* Author:  Leonardo Trevisan Silio
 * Date:    01/01/2024
 */
using System;
using System.Reflection;
using System.Collections.Generic;

namespace Blindness.Internal;

using Exceptions;
using Concurrency;

/// <summary>
/// Dependency injection system.
/// </summary>
public class DependencySystem
{
    private DependencySystem(IAsyncModel model)
        => this.model = model;
    private static DependencySystem crr = null;
    public static DependencySystem Current => crr;

    /// <summary>
    /// Reset a Dependency system sending a model used to init all nodes.
    /// </summary>
    public static void Reset(IAsyncModel model)
        => crr = new(model);
    
    private IAsyncModel model;
    private Assembly crrAssembly = null;
    private Dictionary<Type, Type> typeMap = new();
    
    /// <summary>
    /// Update assembly type used to find concrete types.
    /// </summary>
    public void UpdateAssembly(Assembly assembly)
    {
        this.crrAssembly = assembly;
        this.typeMap = new();
    }

    /// <summary>
    /// Get a concrete object of a Node based on your type.
    /// </summary>
    public Node GetConcrete(Type type)
    {
        try
        {
            var concreteType = findConcrete(type);
            var obj = Activator.CreateInstance(concreteType);

            var node = obj as Node;
            if (node is null)
                return null;
            
            node.Model = this.model;
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

    private Type findConcrete(Type inputType)
    {
        if (this.crrAssembly is not null)
            return findConcreteByAssembly(inputType);
        
        if (!inputType.IsAbstract && !inputType.IsInterface)
            return inputType;

        if (typeMap.ContainsKey(inputType))
            return typeMap[inputType];

        var assembly = inputType.Assembly;
        var types = assembly.GetTypes();

        foreach (var type in types)
        {
            if (!type.Implements(inputType))
                continue;
            
            if (type.GetCustomAttribute<ConcreteAttribute>() is null)
                continue;
            
            this.typeMap.Add(inputType, type);
            return type;
        }

        throw new MissingConcreteTypeException(inputType);
    }

    private Type findConcreteByAssembly(Type inputType)
    {
        if (typeMap.ContainsKey(inputType))
            return typeMap[inputType];

        var types = crrAssembly.GetTypes();

        foreach (var type in types)
        {
            if (!type.Implements(inputType.Name))
                continue;
            
            if (type.GetCustomAttribute<ConcreteAttribute>() is null)
                continue;
            
            this.typeMap.Add(inputType, type);
            return type;
        }

        throw new MissingConcreteTypeException(inputType);
    }
}