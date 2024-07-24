/* Author:  Leonardo Trevisan Silio
 * Date:    23/07/2024
 */
using System;
using System.Reflection;
using System.Collections.Generic;

namespace Blindness.Injection;

using System.IO;
using System.Linq;
using Exceptions;

/// <summary>
/// Dependency injection system.
/// </summary>
public class DependencySystem(Assembly assembly = null)
{
    private static DependencySystem shared = new();

    /// <summary>
    /// Get a global DependencySystem reference.
    /// </summary>
    public static DependencySystem Shared => shared;

    /// <summary>
    /// Reset the Shared Dependency system based in a assembly. If assembly
    /// is null, the entry assembly (Assembly.GetEntryAssembly()) is used.
    /// </summary>
    public static void Reset(Assembly assembly = null)
        => shared = new(assembly);

    readonly Assembly currentAssembly = 
        assembly ?? Assembly.GetEntryAssembly();
    readonly Dictionary<Type, Type> typeMap = [];

    /// <summary>
    /// Get all types that implements a baseType.
    /// </summary>
    public IEnumerable<Type> GetAllTypes(Type baseType)
    {
        ArgumentNullException.ThrowIfNull(baseType, nameof(baseType));

        var types = assembly.GetTypes();
        foreach (var type in types)
        {
            if (!type.Implements(baseType.Name))
                continue;
            
            yield return type;
        }
    }

    /// <summary>
    /// Get all types that implements a baseType.
    /// </summary>
    public IEnumerable<Type> GetAllTypes<T>()
        => GetAllTypes(typeof(T));

    public object Get(Type baseType)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Get a concrete object of a type marked with ConcreteAttribute.
    /// </summary>
    public T GetConcrete<T>(Type type)
        => (T)GetConcrete(type);
    
    /// <summary>
    /// Get a concrete object of a type marked with ConcreteAttribute.
    /// </summary>
    public T GetConcrete<T>()
    {
        var type = typeof(T);
        return (T)GetConcrete(type);
    }

    /// <summary>
    /// Get a concrete object of a type marked with ConcreteAttribute.
    /// </summary>
    public object GetConcrete(Type type)
    {
        ArgumentNullException.ThrowIfNull(type, nameof(type));

        try
        {
            var concreteType = FindConcrete(type);
            var obj = Activator.CreateInstance(concreteType);
            return obj;
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

    object Get(
        Type dep,
        TypeFilterCollection filters,
        TypeList parents,
        TypeNode last)
    {
        var types = GetAllTypes(dep)
            .Where(filters.Filter)
            .ToList();
        
        if (types.Count == 0)
            throw new MissingConcreteTypeException(dep);
        
        if (types.Count > 1)
            throw new ManyConcreteTypeException(dep);
        
        throw new NotImplementedException();
    }

    Type FindConcrete(Type inputType)
    {
        if (typeMap.TryGetValue(inputType, out Type type))
            return type;
        
        if (currentAssembly is not null)
            return FindAndMapType(inputType, currentAssembly);
        
        if (!inputType.IsAbstract && !inputType.IsInterface)
            return inputType;
        
        return FindAndMapType(inputType, inputType.Assembly);
    }

    Type FindAndMapType(Type inputType, Assembly assembly)
    {
        var findedType = inputType.FindConcreteByAssembly(typeof(ConcreteAttribute), assembly);
        typeMap.Add(inputType, findedType);
        return findedType;
    }
}

file class TypeList
{
    TypeNode first = null;
    TypeNode last = null;

    public TypeNode Add(Type type)
    {
        TypeNode node = new(type);
        if (first is null)
            return first = last = node;

        node.Previous = last;
        last.Next = node;
        last = last.Next;
        return node;
    }

    public void Cut(TypeNode node)
    {
        last = node.Previous;
        node.Previous = null;
        last.Next = null;
    }
} 

file class TypeNode(Type value)
{
    public Type Value => value;
    public TypeNode Next { get; set; }
    public TypeNode Previous { get; set; }
}