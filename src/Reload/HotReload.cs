/* Author:  Leonardo Trevisan Silio
 * Date:    11/07/2024
 */
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Reflection;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Blindness.Reload;

using States;
using Injection;
using Concurrency;

/// <summary>
/// HotReload system with the check updates as characterisc operations.
/// </summary>
public class HotReload(IAsyncModel model) : BaseAsyncElement(model)
{   
    int updates = 0;
    bool running = false;
    bool paused = false;
    FileSystemWatcher watcher;
    
    public override void Run()
    {
        running = true;

        InitWatcher();
        int lastUpdates = 0;

        while (running)
        {
            while (paused)
                Thread.Sleep(500);

            if (updates == 0)
            {
                Thread.Sleep(500);
                SendSignal(SignalArgs.False);
                continue;
            }
            
            if (updates > lastUpdates)
            {
                lastUpdates = updates;
                Thread.Sleep(500);
                SendSignal(SignalArgs.False);
                continue;
            }
            
            updates = lastUpdates = 0;
            var newAssembly = GetNewAssembly();
            if (newAssembly is not null)
                UpdateObjects(newAssembly);
            SendSignal(SignalArgs.True);
        }
    }

    public override void Stop()
        => running = false;

    void InitWatcher()
    {
        watcher = new() {
            Path = Environment.CurrentDirectory,
            IncludeSubdirectories = true
        };
        watcher.Filters.Add("*.cs");

        void onChange(object sender, FileSystemEventArgs e)
            => updates++;

        watcher.Created += onChange;
        watcher.Changed += onChange;
        watcher.EnableRaisingEvents = true;
    }

    /// <summary>
    /// Update Current Assembly Code.
    /// </summary>
    public static void UpdateObjects(Assembly assembly)
    {
        if (assembly is null)
            throw new ArgumentNullException(nameof(assembly));
        
        DependencySystem.Shared.UpdateAssembly(assembly);
        Memory.Current.Reload();
    }
    
    static IEnumerable<string> FindAllCSharpFiles(string directory)
    {
        var files = Directory.GetFiles(
            directory, "*.cs", 
            SearchOption.AllDirectories
        );
        
        var codeFiles = 
            from file in files
            where !file.Contains("\\bin\\")
            where !file.Contains("\\obj\\")
            where !file.Contains("/bin/")
            where !file.Contains("/obj/")
            select file;
        
        foreach (var file in codeFiles.Distinct())
            yield return file;
    }

    static IEnumerable<MetadataReference> GetReferences()
    {
        var assembly = Assembly.GetEntryAssembly();
        var assemblies = assembly
            .GetReferencedAssemblies()
            .Select(Assembly.Load)
            .Append(assembly)
            .Append(Assembly.Load("System.Private.CoreLib"));
        
        return assemblies
            .Select(a => MetadataReference.CreateFromFile(a.Location));
    }

    static Assembly GetNewAssembly()
    {
        var files = FindAllCSharpFiles(Environment.CurrentDirectory);
        var syntaxTrees = files
            .Select(File.ReadAllText)
            .Select(text => CSharpSyntaxTree.ParseText(text));

        var compilationOptions = new CSharpCompilationOptions(
            OutputKind.ConsoleApplication
        );
        
        var compilation = CSharpCompilation.Create(
            "HotReloadAppend",
            syntaxTrees: syntaxTrees,
            references: GetReferences(),
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

    public override void Pause()
        => paused = true;

    public override void Resume()
        => paused = false;
}