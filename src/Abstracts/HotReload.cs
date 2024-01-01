/* Author:  Leonardo Trevisan Silio
 * Date:    01/01/2024
 */
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Reflection;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Blindness.Abstracts;

using States;
using Internal;
using Concurrency;

/// <summary>
/// HotReload system.
/// </summary>
public class HotReload : IAsyncElement
{
    private FileSystemWatcher watcher;
    private int updates = 1;
    private bool running = false;
    private AutoResetEvent signal = new(false);

    void updateObjects()
    {
        var assembly = updateAssembly();
        if (assembly is null)
            return;
        
        DependencySystem.Current.UpdateAssembly(assembly);
        Memory.Current.Reload();
    }

    Assembly updateAssembly()
    {
        var sourceFiles = findCSharpFiles(Environment.CurrentDirectory);

        var syntaxTrees = sourceFiles
            .Select(file => CSharpSyntaxTree.ParseText(File.ReadAllText(file)));

        var compilationOptions = new CSharpCompilationOptions(
            OutputKind.ConsoleApplication
        );
        
        var compilation = CSharpCompilation.Create(
            "HotReloadAppend",
            syntaxTrees: syntaxTrees,
            references: getReferences(),
            options: compilationOptions
        );

        using var ms = new MemoryStream();
        var result = compilation.Emit(ms);

        if (result.Success)
        {
            ms.Seek(0, SeekOrigin.Begin);
            return Assembly.Load(ms.ToArray());
        }
        
        foreach (var diagnostic in result.Diagnostics)
            Verbose.Error(diagnostic);
        return null;
    }

    IEnumerable<MetadataReference> getReferences()
    {
        var assembly = Assembly.GetEntryAssembly();
        var assemblies = assembly
            .GetReferencedAssemblies()
            .Select(r => Assembly.Load(r))
            .Append(assembly)
            .Append(Assembly.Load("System.Private.CoreLib"));
        
        return assemblies
            .Select(r => MetadataReference.CreateFromFile(r.Location));
    }

    void initWatcher()
    {
        watcher = new FileSystemWatcher(
            Environment.CurrentDirectory
        );
        watcher.IncludeSubdirectories = true;
        watcher.Filters.Add("*.cs");
        watcher.Filters.Add("*.g.cs");

        FileSystemEventHandler onChange = (sender, e) 
            => updates++;

        watcher.Created += onChange;
        watcher.Changed += onChange;
        watcher.EnableRaisingEvents = true;
    }

    static IEnumerable<string> findCSharpFiles(string directory)
    {
        var files =
            Directory.GetFiles(directory)
            .Where(file => file.EndsWith(".cs"))
            .Where(d => !d.Contains("/obj/"));
        
        foreach (var file in files)
            yield return file;
        
        var directories = Directory
            .GetDirectories(directory);

        foreach (var dir in directories)
        {
            files = findCSharpFiles(dir);
            foreach (var file in files)
                yield return file;
        }
    }

    public void Start()
    {
        running = true;

        initWatcher();
        int lastUpdates = 0;

        while (running)
        {
            if (updates == 0)
            {
                Thread.Sleep(500);
                signal.Set();
                continue;
            }
            
            if (updates > lastUpdates)
            {
                lastUpdates = updates;
                Thread.Sleep(500);
                signal.Set();
                continue;
            }
            
            updates = lastUpdates = 0;
            updateObjects();
            signal.Set();
        }
    }

    public void Await()
        => signal.WaitOne();

    public void Finish()
        => running = false;
}