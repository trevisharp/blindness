/* Author:  Leonardo Trevisan Silio
 * Date:    25/07/2024
 */
using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace Blindness.Injection;

using Exceptions;

/// <summary>
/// Dependency injection system.
/// </summary>
public class DependencySystem
{
    private static DependencySystem shared = new();

    /// <summary>
    /// Get a global DependencySystem reference.
    /// </summary>
    public static DependencySystem Shared => shared;

    /// <summary>
    /// Reset the Shared Dependency system.
    /// </summary>
    public static void Reset()
        => shared = new();

    List<Assembly> assemblies = [ Assembly.GetEntryAssembly() ];
    readonly Dictionary<Type, Type> typeMap = [];

    /// <summary>
    /// Add a assembly to search types.
    /// </summary>
    public void AddAssembly(Assembly assembly)
    {
        ArgumentNullException.ThrowIfNull(assembly, nameof(assembly));
        assemblies.Add(assembly);
    }
    
    /// <summary>
    /// Remove a assembly to search types.
    /// </summary>
    public void RemoveAssembly(Assembly assembly)
    {
        ArgumentNullException.ThrowIfNull(assembly, nameof(assembly));
        assemblies.Remove(assembly);
    }
    
    /// <summary>
    /// Update or Add a Assembly by another with same name.
    /// </summary>
    public void UpdateAssembly(Assembly assembly)
    {
        ArgumentNullException.ThrowIfNull(assembly, nameof(assembly));

        var removedName = assembly.GetName().Name;
        assemblies.RemoveAll(a => a.GetName().Name == removedName);

        assemblies.Add(assembly);
    }

    /// <summary>
    /// Get all types in the assembly that implements a baseType.
    /// </summary>
    public IEnumerable<Type> GetAllTypes(Type baseType)
    {
        ArgumentNullException.ThrowIfNull(baseType, nameof(baseType));

        return assemblies
            .SelectMany(a => a.GetTypes())
            .Where(type => type.IsAssignableFrom(baseType));
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
        DepFunction function = null,
        TypeFilterCollection filters = null)
    {
        ArgumentNullException.ThrowIfNull(dep, nameof(dep));
        function ??= DepFunction.Constructor;
        filters ??= [];

        var types = GetAllTypes(dep)
            .Where(t => !t.IsAbstract)
            .Where(t => !t.IsInterface)
            .Where(filters.Filter)
            .ToList();
        
        if (types.Count == 0)
            throw new MissingConcreteTypeException(dep);
        
        if (types.Count > 1)
            throw new ManyConcreteTypeException(dep);
        
        var list = new TypeList();
        return Get(types[0], list, null);
    }

    object Get(
        Type type,
        TypeList parents,
        TypeNode last)
    {

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