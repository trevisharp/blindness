/* Author:  Leonardo Trevisan Silio
 * Date:    23/07/2024
 */
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace Blindness.Factory;

using Utils;

/// <summary>
/// Code generator used to implement concrete types automatically.
/// </summary>
public abstract class Implementer(Type baseType)
{
    public List<BaseTypeImplementation> BaseTypeImplementations { get; set; } = [];
    public List<ExtraFile> ExtraFiles { get; set; } = [];

    /// <summary>
    /// Try implement a type and his cache if needed.
    /// </summary>
    public virtual void Implement()
    {
        Verbose.Info("Generating Files...");

        var nonConstDir = Path.Combine(
            Environment.CurrentDirectory,
            "NonConstantGeneratedFiles"
        );
        var cachePath = Path.Combine(
            nonConstDir, ".cache"
        );
        if (!Directory.Exists(nonConstDir))
            Directory.CreateDirectory(nonConstDir);
        
        var constDir = Path.Combine(
            Environment.CurrentDirectory,
            "ConstantGeneratedFiles"
        );
        var hasConstantFiles = ExtraFiles?.Exists(e => e.Constant) ?? false;
        if (!Directory.Exists(constDir) && hasConstantFiles)
            Directory.CreateDirectory(constDir);
        
        var cache = LoadCache(cachePath);

        var assembly = Assembly.GetEntryAssembly();
        var nodes = FindValidNodes(assembly, baseType);
        foreach (var node in nodes)
            Implement(nonConstDir, node, cache);
            
        foreach (var extraFile in ExtraFiles ?? [])
        {
            var path = Path.Combine(
                extraFile.Constant ? constDir : nonConstDir, 
                extraFile.FileName
            );

            if (extraFile.Constant && File.Exists(path))
                continue;
            var content = extraFile.Get();

            if (cache.TryGetValue(path, out string value) && value == content.ToHash())
                continue;
            File.WriteAllText(path, content);
        }
        
        SaveCache(cachePath, cache);
    }

    /// <summary>
    /// Implement a files based on GetCode behavior.
    /// Use the file content to save .cache file.
    /// The generated file has name type.Name + 'Concrete.g.cs'.
    /// </summary>
    protected virtual void Implement(string dirPath, Type type, Dictionary<string, string> cache)
    {
        var filePath = Path.Combine(
            dirPath, $"{type.Name}Concrete.g.cs"
        );
        var nodeCode = GetCode(type, filePath);

        var codeHash = nodeCode.ToHash();
        bool hasKey = cache.ContainsKey(filePath);
        if (hasKey && cache[filePath] == codeHash)
            return;
        
        cache[filePath] = codeHash;
        File.WriteAllText(filePath, nodeCode);
    }

    /// <summary>
    /// Get code based in all implementations of a type.
    /// </summary>
    protected virtual string GetCode(Type baseType, string fileName)
    {
        var builder = new ClassBuilder();
        var properties = GetProperties(baseType);
        var methods = GetMethods(baseType);

        foreach (var implementation in BaseTypeImplementations ?? [])
        {
            implementation.ImplementType(
                builder, fileName, baseType,
                properties, methods
            );
        }

        return builder.Build();
    }

    /// <summary>
    /// Save the cache dictionary in a file.
    /// </summary>
    protected static void SaveCache(string cachePath, Dictionary<string, string> cache)
    {
        File.WriteAllLines(
            cachePath,
            cache.Select(x => $"{x.Key} {x.Value}")
        );
    }

    /// <summary>
    /// Load the cache file dictionary.
    /// </summary>
    protected static Dictionary<string, string> LoadCache(string cachePath)
    {
        return
            !File.Exists(cachePath) ? [] :
            File.ReadAllLines(cachePath)
                .Select(line => line.Split(' '))
                .ToDictionary(
                    data => data[0],
                    data => data[1]
                );
    }

    /// <summary>
    /// Get all methods defined by a type or his base interfaces.
    /// </summary>
    protected static List<MethodInfo> GetMethods(Type type)
    {
        var types = GetAllBaseTypes(type);
        return types
            .Append(type)
            .SelectMany(t => t.GetMethods())
            .ToList();
    }

    /// <summary>
    /// Get all properties defined by a type or his base interfaces.
    /// </summary>
    protected static List<PropertyInfo> GetProperties(Type type)
    {
        var types = GetAllBaseTypes(type);
        return types
            .Append(type)
            .SelectMany(t => t.GetProperties())
            .ToList();
    }
    
    /// <summary>
    /// Get all implemented/inherited types.
    /// </summary>
    protected static List<Type> GetAllBaseTypes(Type type)
    {
        List<Type> types = [.. type.GetInterfaces()];
        if (type.BaseType is not null)
            types.Add(type.BaseType);
        
        for (int i = 0; i < types.Count; i++)
            types.AddRange(
                GetAllBaseTypes(types[i])
            );
        
        return types;
    }

    /// <summary>
    /// Find all abstract types that implments/inherits a specific baseType in a assembly.
    /// Ignore all types marked with Ignore Attribute.
    /// </summary>
    protected static Type[] FindValidNodes(Assembly assembly, Type baseType)
    {
        var types = assembly.GetTypes();
        var dependencies = GetAllAbstractDependenies(types);
        List<Type> validBaseType = [ baseType ];
        bool needContinue = true;

        while (needContinue)
        {
            needContinue = false;
            for (int i = 0; i < dependencies.Count; i++)
            {
                var type = dependencies[i];
                if (!ImplementsOrInherits(type, validBaseType))
                    continue;
                
                dependencies.Remove(type);
                needContinue = true;
                validBaseType.Add(type);
                i--;
            }
        }

        return [ 
            ..from type in validBaseType
            where type.Assembly == assembly
            where type.GetCustomAttribute<IgnoreAttribute>() is null
            select type
        ];
    }

    /// <summary>
    /// Return true if a type implements any type of a collection of types.
    /// </summary>
    protected static bool ImplementsOrInherits(Type type, IEnumerable<Type> baseTypes)
    {
        var interfaces = type.GetInterfaces();
        if (interfaces.Any(baseTypes.Contains))
            return true;
        
        return baseTypes.Contains(type.BaseType);
    }

    /// <summary>
    /// Get all types are inherited/implemented by any type in a colletion of types.
    /// </summary>
    protected static List<Type> GetAllAbstractDependenies(IEnumerable<Type> types)
    {
        Queue<Type> typeQueue = new(types);
        List<Type> dependecies = [];

        while (typeQueue.Count > 0)
        {
            var type = typeQueue.Dequeue();
            if (dependecies.Contains(type))
                continue;
            
            if (type.IsAbstract || type.IsInterface)
                dependecies.Add(type);
            
            var baseInterfaces = type.GetInterfaces();
            foreach (var baseType in baseInterfaces)
                typeQueue.Enqueue(baseType);
            
            if (type.BaseType is not null)
                typeQueue.Enqueue(type.BaseType);
        }
        
        return dependecies;
    }
}