using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Blindness.Abstracts;

using States;
using Internal;

public static class HotReload
{
    private static Dictionary<string, List<CallInfo>> infos;
    private static FileSystemWatcher watcher;
    
    public static bool IsActive 
    {
        get => watcher?.EnableRaisingEvents ?? false;
        set
        {
            if (watcher is not null)
            {
                watcher.EnableRaisingEvents = value;
                return;
            }

            if (!value)
                return;

            initWatcher();
        }
    }

    static HotReload()
    {
        infos = new();
        IsActive = false;
    }

    public static bool Use(
        object obj,
        string code,
        MethodInfo method,
        params object[] parameters
    )
    {
        if (!IsActive)
            return false;
        
        var infos = get(code);
        CallInfo callInfo = infos
            .FirstOrDefault(i => i.OriginalMethod == method);
        
        if (callInfo is null)
        {
            callInfo = new CallInfo(method);
            infos.Add(callInfo);
            return false;
        }

        if (callInfo.CurrentMethod == method)
            return false;

        callInfo.Call(obj, parameters);
        return true;
    }

    static void updateInfos(string file)
    {
        Verbose.Info(file + " updated! Applying hot reload...");
        watcher.EnableRaisingEvents = false;

        var assembly = updateAssembly();
        if (assembly is null)
            return;
        
        var code = file.ToHash();
        var infos = get(code);
        

        var types = assembly.GetTypes();
        foreach (var info in infos)
        {
            var type = types
                .FirstOrDefault(t => t.Name == info.Type.Name);
            if (type is null)
                continue;
            
            var methods = type.GetRuntimeMethods();
            var method = methods
                .FirstOrDefault(m => m.Name == info.OriginalMethod.Name);
            if (method is null)
                continue;

            Verbose.Info($"Updating {info.OriginalMethod.Name}...");
            info.CurrentMethod = method;
        }

        watcher.EnableRaisingEvents = true;
    }

    static List<CallInfo> get(string code)
    {
        if (infos.ContainsKey(code))
            return infos[code];
        
        var newItem = new List<CallInfo>();
        infos.Add(code, newItem);
        return newItem;
    }

    static Assembly updateAssembly()
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

    static IEnumerable<MetadataReference> getReferences()
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

    static void initWatcher()
    {
        watcher = new FileSystemWatcher(
            Environment.CurrentDirectory
        );
        watcher.IncludeSubdirectories = true;
        watcher.Filters.Add("*.cs");
        watcher.Changed += (sender, e) =>
        {
            try
            {
                updateInfos(e.FullPath);
            }
            catch (Exception ex)
            {
                Verbose.Error(ex.Message);
                Verbose.Error(ex.StackTrace);
            }
        };
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
}