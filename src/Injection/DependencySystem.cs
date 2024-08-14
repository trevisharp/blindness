/* Author:  Leonardo Trevisan Silio
 * Date:    14/08/2024
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

    readonly List<Assembly> assemblies = [ Assembly.GetEntryAssembly() ];
    readonly Dictionary<Type, Type> typeCache = [];

    /// <summary>
    /// Find a type on the list of assemblies that has a type with
    /// same name and namespaces that given type.
    /// </summary>
    public Type GetCompatibilityType(Type type)
    {
        ArgumentNullException.ThrowIfNull(type, nameof(type));
        if (assemblies.Contains(type.Assembly))
            return type;
        
        var compatibilityType = assemblies
            .SelectMany(a => a.GetTypes())
            .FirstOrDefault(t => t.FullName == type.FullName);
        
        return compatibilityType;
    }

    /// <summary>
    /// Add a assembly to search types.
    /// </summary>
    public void AddAssembly(Assembly assembly)
    {
        ArgumentNullException.ThrowIfNull(assembly, nameof(assembly));
        assemblies.Add(assembly);
        typeCache.Clear();
    }
    
    /// <summary>
    /// Remove a assembly to search types.
    /// </summary>
    public void RemoveAssembly(Assembly assembly)
    {
        ArgumentNullException.ThrowIfNull(assembly, nameof(assembly));

        if (assemblies.Count < 2)
            return;

        assemblies.Remove(assembly);
        typeCache.Clear();
    }
    
    /// <summary>
    /// Update the main assembly from the assembly list.
    /// </summary>
    public void UpdateAssembly(Assembly assembly)
    {
        ArgumentNullException.ThrowIfNull(assembly, nameof(assembly));
        if (assemblies.Count == 0)
            assemblies.Add(assembly);
        else assemblies[0] = assembly;
        typeCache.Clear();
    }

    /// <summary>
    /// Get all types in the assembly that implements a baseType.
    /// </summary>
    public IEnumerable<Type> GetAllTypesByBaseType(Type baseType)
    {
        ArgumentNullException.ThrowIfNull(baseType, nameof(baseType));

        return assemblies
            .SelectMany(a => a.GetTypes())
            .Where(type => type.IsAssignableTo(baseType));
    }

    /// <summary>
    /// Get all types that implements a baseType.
    /// </summary>
    public IEnumerable<Type> GetAllTypesByBaseType<T>()
        => GetAllTypesByBaseType(typeof(T));
    
    /// <summary>
    /// Instanciate a type based on injection arguments.
    /// </summary>
    public object Get(Type type, InjectionArgs args)
    {
        ArgumentNullException.ThrowIfNull(type, nameof(type));
        ArgumentNullException.ThrowIfNull(args, nameof(args));

        if (type.IsAbstract || type.IsInterface)
            type = FindConcreteType(type, args);

        if (args.DependencyGraph.Contains(type))
            throw new CycleDependencyException(type);

        args.DependencyGraph.Push(type);
        var obj = args.DepFunction.Call(type, this, args);
        args.DependencyGraph.Pop();

        return obj;
    }

    /// <summary>
    /// Instanciate a type based on type filter for abstract types.
    /// </summary>
    public object Get(Type type, DepFunction function, TypeFilterCollection filter)
    {
        ArgumentNullException.ThrowIfNull(filter, nameof(filter));
        ArgumentNullException.ThrowIfNull(function, nameof(function));
        return Get(type, new InjectionArgs(function) {
            Filters = filter
        });
    }

    /// <summary>
    /// Instanciate a type based on dependency function.
    /// </summary>
    public object Get(Type type, DepFunction function)
        => Get(type, function, []);

    /// <summary>
    /// Instanciate a type based on type filter for abstract types.
    /// </summary>
    public object Get(Type type, TypeFilterCollection filter)
        => Get(type, DepFunction.Constructor, filter);

    /// <summary>
    /// Instanciate a type based on default injection arguments with constructor.
    /// </summary>
    public object Get(Type type)
        => Get(type, InjectionArgs.Default);
    
    /// <summary>
    /// Instanciate a type based on injection arguments.
    /// </summary>
    public T Get<T>(InjectionArgs args)
        => (T)Get(typeof(T), args);

    /// <summary>
    /// Instanciate a type based on dependency function and filters.
    /// </summary>
    public T Get<T>(DepFunction function, TypeFilterCollection filter)
        => (T)Get(typeof(T), function, filter);
    
    /// <summary>
    /// Instanciate a type based on dependency function.
    /// </summary>
    public T Get<T>(DepFunction function)
        => (T)Get(typeof(T), function);

    /// <summary>
    /// Instanciate a type based on filters.
    /// </summary>
    public T Get<T>(TypeFilterCollection filter)
        => (T)Get(typeof(T), filter);

    /// <summary>
    /// Instanciate a type based on default injection arguments with constructor.
    /// </summary>
    public T Get<T>()
        => (T)Get(typeof(T), InjectionArgs.Default);
    
    Type FindConcreteType(Type dep, InjectionArgs args)
    {
        ArgumentNullException.ThrowIfNull(dep, nameof(dep));
        ArgumentNullException.ThrowIfNull(args, nameof(args));
        ArgumentNullException.ThrowIfNull(args.Filters, nameof(args.Filters));
        
        if (typeCache.TryGetValue(dep, out Type type))
            return type;

        var types = GetAllTypesByBaseType(dep)
            .Where(t => !t.IsAbstract)
            .Where(t => !t.IsInterface)
            .Where(args.Filters.Filter)
            .ToList();
        
        if (types.Count == 0)
            throw new MissingConcreteTypeException(dep);
        
        if (types.Count > 1)
            throw new ManyConcreteTypeException(dep);
        
        var finded = types[0];
        typeCache.Add(dep, finded);
        return finded;
    }
}