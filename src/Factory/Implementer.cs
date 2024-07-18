/* Author:  Leonardo Trevisan Silio
 * Date:    15/07/2024
 */
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace Blindness.Factory;

using Utils;

/// <summary>
/// Code generator used to implement concrete nodes automatically.
/// </summary>
public abstract class Implementer
{
    public Type BaseInterface { get; protected set; }
    public List<Implementation> Implementations { get; protected set; }
    public List<ExtraFile> ExtraFiles { get; protected set; }

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
        var nodeTypes = FindValidNodeType(assembly);
        foreach (var node in nodeTypes)
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
    /// </summary>
    protected virtual void Implement(string dirPath, Type type, Dictionary<string, string> cache)
    {
        var filePath = Path.Combine(
            dirPath,
            $"{type.Name}Concrete.g.cs"
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
    protected virtual string GetCode(Type baseInterface, string fileName)
    {
        var builder = new ClassBuilder();
        var properties = GetInterfaceProperties(baseInterface);
        var methods = GetIntefaceMethods(baseInterface);

        foreach (var implementation in Implementations ?? [])
        {
            implementation.ImplementType(
                builder,
                fileName, baseInterface,
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
    protected static List<MethodInfo> GetIntefaceMethods(Type type)
    {
        var types = GetAllBaseInterfaces(type);
        return types
            .Append(type)
            .SelectMany(t => t.GetMethods())
            .ToList();
    }

    /// <summary>
    /// Get all properties defined by a type or his base interfaces.
    /// </summary>
    protected static List<PropertyInfo> GetInterfaceProperties(Type type)
    {
        var types = GetAllBaseInterfaces(type);
        return types
            .Append(type)
            .SelectMany(t => t.GetProperties())
            .Where(p => p.Name != "Bind")
            .ToList();
    }
    
    /// <summary>
    /// Get all implemented/inherited interface of a type.
    /// </summary>
    protected static List<Type> GetAllBaseInterfaces(Type type)
    {
        List<Type> types = [.. type.GetInterfaces()];
        
        for (int i = 0; i < types.Count; i++)
            types.AddRange(
                GetAllBaseInterfaces(types[i])
            );
        
        return types;
    }

    /// <summary>
    /// Find all interface types that implments/inherits
    /// INode and not has Ignore attribute in a assembly.
    /// </summary>
    protected Type[] FindValidNodeType(Assembly assembly)
    {
        var types = assembly.GetTypes();
        var interfaces = GetAllDependentInterfaces(types);
        List<Type> nodeTypes = [ BaseInterface ];
        bool needContinue = true;

        while (needContinue)
        {
            needContinue = false;
            for (int i = 0; i < interfaces.Count; i++)
            {
                var type = interfaces[i];
                if (!ImplementsOrInheritsInterface(type, nodeTypes))
                    continue;
                
                interfaces.Remove(type);
                needContinue = true;
                nodeTypes.Add(type);
                i--;
            }
        }

        return [ 
            ..from node in nodeTypes
            where node.Assembly == assembly
            where node.GetCustomAttribute<IgnoreAttribute>() is null
            select node
        ];
    }

    /// <summary>
    /// Return true if a type implements any interface of a collection of types.
    /// </summary>
    protected static bool ImplementsOrInheritsInterface(Type type, IEnumerable<Type> interfaces)
    {
        var baseTypes = type.GetInterfaces();
        return baseTypes.Any(interfaces.Contains);
    }

    /// <summary>
    /// Get all interfaces a collection of types implements.
    /// </summary>
    protected static List<Type> GetAllDependentInterfaces(IEnumerable<Type> types)
    {
        Queue<Type> typeQueue = new(types);
        List<Type> interfaces = [];

        while (typeQueue.Count > 0)
        {
            var type = typeQueue.Dequeue();
            if (interfaces.Contains(type))
                continue;
            
            if (type.IsInterface)
                interfaces.Add(type);
            
            var baseInterfaces = type.GetInterfaces();
            foreach (var baseType in baseInterfaces)
                typeQueue.Enqueue(baseType);
        }
        
        return interfaces;
    }
}