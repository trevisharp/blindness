using System;
using System.Linq;
using System.Reflection;
using System.Diagnostics;
using System.Collections.Generic;

namespace Blindness.Abstracts;

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

    private void implements()
    {
        Verbose.Info("Implementing Concrete Nodes...");
        var nodeTypes = getNodeType();

        foreach (var node in nodeTypes)
            Verbose.Info(node);
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

    private void reRun()
    {
        Verbose.Info("Rebuilding the app...", 1);
        execute("dotnet", "build");
        
        var assembly = Assembly.GetEntryAssembly();
        var dll = assembly.Location;
        var exe = dll.Replace(".dll", ".exe");
        execute(exe);
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

    private bool needImplement()
    {
        return Random.Shared.Next(2) == 0;
    }
}