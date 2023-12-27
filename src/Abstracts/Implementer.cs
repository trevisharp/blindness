using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Diagnostics;
using System.Collections.Generic;

namespace Blindness.Abstracts;

using Internal;

public class Implementer
{
    public void ImplementAndRun(Action appFunc)
    {
        if (!needImplement())
        {
            appFunc();
            return;
        }

        implements();
        reRun();
    }

    private bool needImplement()
    {
        return Random.Shared.Next(2) == 0;
    }

    private void implements()
    {
        Verbose.Info("Implementing Concrete Nodes...");
        var nodeTypes = getNodeType();

        foreach (var node in nodeTypes)
            implement(node);
    }
    
    private void reRun()
    {
        Verbose.Info("Rebuilding the app...", 1);
        CSharpCompiler.Recompile();
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

    private string getCode(Type type, string fileName)
    {
        var code = fileName.ToHash();
        StringBuilder fieldsMapCode = new();
        StringBuilder fieldsCode = new();
        StringBuilder methodsCode = new();

        var props = getInterfaceProperties(type);
        int fieldCount = props.Count;

        var methods = getIntefaceMethods(type);

        var deps = methods
            .FirstOrDefault(m => m.Name == "Deps");
        if (deps is not null)
        {
            var parameters = deps.GetParameters();
            methodsCode.AppendLine("public void Deps(");
            for (int i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];
                methodsCode.Append(
                    $"\t{parameter.ParameterType.Name} {parameter.Name}"
                );
                if (i < parameters.Length - 1)
                    methodsCode.Append(",");
                methodsCode.AppendLine();
            }
            methodsCode.AppendLine(")");
            methodsCode.AppendLine("{");
            
            for (int i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];
                methodsCode.AppendLine(
                    $"this.{parameter.Name} = {parameter.Name};"
                );
            }
            methodsCode.AppendLine("}");

            methods.Remove(deps);
        }
        
        var onLoad = methods
            .FirstOrDefault(m => m.Name == "OnLoad");
        if (onLoad is not null)
        {
            methodsCode.AppendLine(
                $$"""
                protected override void OnLoad()
                {
                    var hotReloaded = Blindness.Abstracts.HotReload.Use(
                        "{{type.Name}}Concrete.OnLoad",
                        "{{code}}"
                    );
                    if (hotReloaded)
                        return;
                    
                    (({{type.Name}})this).OnLoad();
                }
                """
            );
            methods.Remove(onLoad);
        }
        
        var onRun = methods
            .FirstOrDefault(m => m.Name == "OnProcess");
        if (onRun is not null)
        {
            methodsCode.AppendLine(
                $$"""
                protected override void OnRun()
                {
                    var hotReloaded = Blindness.Abstracts.HotReload.Use(
                        "{{type.Name}}Concrete.OnLoad",
                        {{code}}
                    );
                    if (hotReloaded)
                        return;
                    
                    (({{type.Name}})this).OnProcess();
                }
                """
            );
            methods.Remove(onRun);
        }

        for (int i = 0; i < props.Count; i++)
        {
            var prop = props[i];
            fieldsMapCode.AppendLine(
                $"\t\t\"{prop.Name}\" => {i},"
            );

            fieldsCode.AppendLine(
                $$"""

                    public {{prop.PropertyType.Name}} {{prop.Name}}
                    {
                        get => Bind.Get<{{prop.PropertyType.Name}}>({{i}});
                        set => Bind.Set({{i}}, value);
                    }
                """
            );
            i++;
        }
        fieldsMapCode.Append("\t\t_ => -1");
        
        return
        $$"""
        using Blindness;
        using Blindness.States;

        [Concrete]
        public class {{type.Name}}Concrete : Node, {{type.Name}}
        {
            public {{type.Name}}Concrete() =>
                this.Bind = new Binding(
                    this, {{fieldCount}}, typeof({{type.Name}}),
                    s => s switch
                    {
                {{fieldsMapCode}}
                    }
                );

            {{fieldsCode}}

            {{methodsCode}}
        }
        """;
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

    private void execute(string filename, string args = "")
    {
        var info = new ProcessStartInfo {
            FileName = filename,
            Arguments = args,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = false
        };

        var process = new Process {
            StartInfo = info
        };

        process.ErrorDataReceived += (o, e) =>
            Verbose.Error(e.Data, 1);
        process.OutputDataReceived += (o, e) =>
            Verbose.Content(e.Data, 1);

        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        process.WaitForExit();
    }
}