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
        var produced = new HashSet<string>();

        var assembly = Assembly.GetEntryAssembly();
        var nodes = FindValidNodes(assembly, baseType);
        foreach (var node in nodes)
        {
            Implement(nonConstDir, node, cache);
            produced.Add(GetFilePath(nonConstDir, node));
        }
            
        foreach (var extraFile in ExtraFiles ?? [])
        {
            var path = Path.Combine(
                extraFile.Constant ? constDir : nonConstDir, 
                extraFile.FileName
            );

            if (extraFile.Constant && File.Exists(path))
                continue;
            
            produced.Add(path);
            var content = extraFile.Get();
            var hash = content.ToHash();
            if (cache.TryGetValue(path, out string value) && value == hash && File.Exists(path))
                continue;
            
            cache[path] = hash;
            File.WriteAllText(path, content);
        }

        RemoveOrphanFiles(nonConstDir, cache, produced);
        SaveCache(cachePath, cache);
    }

    /// <summary>
    /// Delete generated files of nodes that no longer exist.
    /// </summary>
    protected static void RemoveOrphanFiles(string dirPath, Dictionary<string, string> cache, HashSet<string> produced)
    {
        var fullDir = Path.GetFullPath(dirPath);
        foreach (var path in cache.Keys.ToList())
        {
            if (produced.Contains(path))
                continue;
            
            cache.Remove(path);
            var isGenerated = Path.GetFullPath(path).StartsWith(fullDir, StringComparison.OrdinalIgnoreCase)
                && path.EndsWith(".g.cs", StringComparison.OrdinalIgnoreCase);
            if (isGenerated && File.Exists(path))
                File.Delete(path);
        }
    }

    /// <summary>
    /// Get the path of the generated file of a type.
    /// </summary>
    protected virtual string GetFilePath(string dirPath, Type type)
    {
        var prefix = string.IsNullOrEmpty(type.Namespace) ? "" : type.Namespace + ".";
        return Path.Combine(dirPath, $"{prefix}{type.Name}Concrete.g.cs");
    }

    /// <summary>
    /// Implement a files based on GetCode behavior.
    /// Use the file content to save .cache file.
    /// </summary>
    protected virtual void Implement(string dirPath, Type type, Dictionary<string, string> cache)
    {
        var filePath = GetFilePath(dirPath, type);
        var nodeCode = GetCode(type, filePath);

        var codeHash = nodeCode.ToHash();
        if (cache.TryGetValue(filePath, out var cached) && cached == codeHash && File.Exists(filePath))
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
            cache.Select(x => $"{x.Key}\t{x.Value}")
        );
    }

    /// <summary>
    /// Load the cache file dictionary.
    /// </summary>
    protected static Dictionary<string, string> LoadCache(string cachePath)
    {
        Dictionary<string, string> cache = [];
        if (!File.Exists(cachePath))
            return cache;

        foreach (var line in File.ReadAllLines(cachePath))
        {
            var separator = line.LastIndexOf('\t');
            if (separator < 0)
                separator = line.LastIndexOf(' ');
            if (separator <= 0)
                continue;
            
            cache[line[..separator]] = line[(separator + 1)..];
        }
        return cache;
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
            .Distinct()
            .ToList();
    }

    /// <summary>
    /// Get all properties defined by a type or his base interfaces.
    /// </summary>
    protected static List<PropertyInfo> GetProperties(Type type)
    {
        var types = GetAllBaseTypes(type);
        return types
            .Prepend(type)
            .SelectMany(t => t.GetProperties())
            .DistinctBy(p => p.Name)
            .ToList();
    }
    
    /// <summary>
    /// Get all implemented/inherited types.
    /// </summary>
    protected static List<Type> GetAllBaseTypes(Type type)
    {
        List<Type> types = [];
        Queue<Type> pending = new();
        pending.Enqueue(type);

        while (pending.Count > 0)
        {
            var current = pending.Dequeue();
            var bases = current.GetInterfaces().AsEnumerable();
            if (current.BaseType is not null)
                bases = bases.Append(current.BaseType);

            foreach (var baseType in bases)
            {
                if (types.Contains(baseType))
                    continue;
                types.Add(baseType);
                pending.Enqueue(baseType);
            }
        }
        
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