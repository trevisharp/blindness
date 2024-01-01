/* Author:  Leonardo Trevisan Silio
 * Date:    01/01/2024
 */
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace Blindness.Abstracts;

/// <summary>
/// Code generator used to implement concrete nodes automatically.
/// </summary>
public abstract class Implementer
{
    public Type BaseConcreteType { get; set; }
    public List<Implementation> Implementations { get; private set; } = new();

    /// <summary>
    /// Try implement if needed.
    /// </summary>
    public void Implement()
    {
        Verbose.Info("Implementing Concrete Nodes...");
        var nodeTypes = getNodeType();

        foreach (var node in nodeTypes)
            implement(node);
    }

    private void implement(Type type)
    {
        var dirPath = Path.Combine(
            Environment.CurrentDirectory,
            "ConcreteNodes"
        );
        var filePath = Path.Combine(
            dirPath,
            $"{type.Name}Concrete.g.cs"
        );

        if (!Directory.Exists(dirPath))
            Directory.CreateDirectory(dirPath);

        var nodeCode = getCode(type, filePath);
        File.WriteAllText(filePath, nodeCode);
    }

    private string getCode(Type baseInterface, string fileName)
    {
        ClassBuilder builder = new ClassBuilder();
        var properties = getInterfaceProperties(baseInterface);
        var methods = getIntefaceMethods(baseInterface);

        foreach (var implementation in this.Implementations)
        {
            implementation.ImplementType(
                builder,
                fileName, baseInterface,
                properties, methods
            );
        }

        return builder.Build();
    }

    private List<MethodInfo> getIntefaceMethods(Type type)
    {
        var types = getAllBaseInterfaces(type);
        return types
            .Append(type)
            .SelectMany(t => t.GetMethods())
            .ToList();

    }

    private List<PropertyInfo> getInterfaceProperties(Type type)
    {
        var types = getAllBaseInterfaces(type);
        return types
            .Append(type)
            .SelectMany(t => t.GetProperties())
            .Where(p => p.Name != "Bind")
            .ToList();
    }

    private List<Type> getAllBaseInterfaces(Type type)
    {
        List<Type> types = type
            .GetInterfaces()
            .ToList();
        
        for (int i = 0; i < types.Count; i++)
            types.AddRange(
                getAllBaseInterfaces(types[i])
            );
        
        return types;
    }

    private Type[] getNodeType()
    {
        var assembly = Assembly.GetEntryAssembly();
        var interfaces = getAllInterfaces(assembly);

        List<Type> nodeTypes = new List<Type>();
        nodeTypes.Add(typeof(INode));
        bool needContinue = true;

        while (needContinue)
        {
            needContinue = false;
            for (int i = 0; i < interfaces.Count; i++)
            {
                bool isNodeType = false;
                var type = interfaces[i];
                var baseTypes = type.GetInterfaces();
                
                foreach (var baseType in baseTypes)
                {
                    if (!nodeTypes.Contains(baseType))
                        continue;

                    isNodeType = true;
                    break;
                }
                if (!isNodeType)
                    continue;
                
                interfaces.Remove(type);
                needContinue = true;
                nodeTypes.Add(type);
                i--;
            }
        }

        return nodeTypes
            .Where(node => node.Assembly == assembly)
            .Where(node => node.GetCustomAttribute<IgnoreAttribute>() is null)
            .ToArray();
    }

    private List<Type> getAllInterfaces(Assembly assembly)
    {
        var types = assembly.GetTypes();
        var queue = new Queue<Type>(
            types.Where(type => type.IsInterface)
        );
        var interfaces = new List<Type>();

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