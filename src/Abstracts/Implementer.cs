/* Author:  Leonardo Trevisan Silio
 * Date:    11/07/2024
 */
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace Blindness.Abstracts;

using Internal;

/// <summary>
/// Code generator used to implement concrete nodes automatically.
/// </summary>
public abstract class Implementer
{
    public Type BaseConcreteType { get; set; }
    public List<Implementation> Implementations { get; set; }
    public List<ExtraFile> ExtraFiles { get; set; }

    /// <summary>
    /// Try implement if needed.
    /// </summary>
    public void Implement()
    {
        Verbose.Info("Generating Files...");
        var assembly = Assembly.GetEntryAssembly();
        var nodeTypes = FindValidNodeType(assembly);
        var nonConstDir = Path.Combine(
            Environment.CurrentDirectory,
            "NonConstantGeneratedFiles"
        );
        var ConstDir = Path.Combine(
            Environment.CurrentDirectory,
            "ConstantGeneratedFiles"
        );
        var cachePath = Path.Combine(
            nonConstDir, ".cache"
        );
        
        if (!Directory.Exists(nonConstDir))
            Directory.CreateDirectory(nonConstDir);
        
        if (!Directory.Exists(ConstDir) && (ExtraFiles?.Exists(e => e.Constant) ?? false))
            Directory.CreateDirectory(ConstDir);
        
        var dict = 
            !File.Exists(cachePath) ? [] :
            File.ReadAllLines(cachePath)
                .Select(line => line.Split(' '))
                .ToDictionary(
                    data => data[0],
                    data => data[1]
                );

        foreach (var node in nodeTypes)
            Implement(nonConstDir, node, dict);
            
        foreach (var extraFile in ExtraFiles ?? [])
        {
            var path = Path.Combine(
                extraFile.Constant ? ConstDir : nonConstDir, 
                extraFile.FileName
            );

            if (extraFile.Constant && File.Exists(path))
                continue;
            var content = extraFile.Get();

            if (dict.TryGetValue(path, out string value) && value == content.ToHash())
                continue;
            File.WriteAllText(path, content);
        }
        
        File.WriteAllLines(
            cachePath,
            dict.Select(x => $"{x.Key} {x.Value}")
        );
    }

    void Implement(string dirPath, Type type, Dictionary<string, string> cache)
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

    string GetCode(Type baseInterface, string fileName)
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

    static List<MethodInfo> GetIntefaceMethods(Type type)
    {
        var types = GetAllBaseInterfaces(type);
        return types
            .Append(type)
            .SelectMany(t => t.GetMethods())
            .ToList();
    }

    static List<PropertyInfo> GetInterfaceProperties(Type type)
    {
        var types = GetAllBaseInterfaces(type);
        return types
            .Append(type)
            .SelectMany(t => t.GetProperties())
            .Where(p => p.Name != "Bind")
            .ToList();
    }

    static List<Type> GetAllBaseInterfaces(Type type)
    {
        List<Type> types = [.. type.GetInterfaces()];
        
        for (int i = 0; i < types.Count; i++)
            types.AddRange(
                GetAllBaseInterfaces(types[i])
            );
        
        return types;
    }

    static Type[] FindValidNodeType(Assembly assembly)
    {
        var interfaces = GetAllDependentInterfaces(assembly);
        List<Type> nodeTypes = [ typeof(INode) ];
        bool needContinue = true;

        while (needContinue)
        {
            needContinue = false;
            for (int i = 0; i < interfaces.Count; i++)
            {
                var type = interfaces[i];
                if (!IsNodeType(type, nodeTypes))
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

    static bool IsNodeType(Type type, List<Type> nodeTypes)
    {
        var baseTypes = type.GetInterfaces();
        return baseTypes.Any(nodeTypes.Contains);
    }

    static List<Type> GetAllDependentInterfaces(Assembly assembly)
    {
        var types = assembly.GetTypes();
        Queue<Type> queue = new(types.Where(type => type.IsInterface));
        List<Type> interfaces = [];

        while (queue.Count > 0)
        {
            var type = queue.Dequeue();
            var baseTypes = type.GetInterfaces();
            
            if (!interfaces.Contains(type))
                interfaces.Add(type);
            
            foreach (var baseType in baseTypes)
                if (!interfaces.Contains(baseType))
                    queue.Enqueue(baseType);
        }
        
        return interfaces;
    }
}