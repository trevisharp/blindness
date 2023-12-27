using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Blindness.Abstracts;

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
        string code,
        string function,
        params object[] parameters
    )
    {
        if (!IsActive)
            return false;

        if (!infos.ContainsKey(code))
        {

        }


        return true;
    }

    static void updateInfos(string file)
    {
        var assembly = compile(file);
        if (assembly is null)
            return;
        
        var code = file.ToHash();
        var info = infos[code];
    }

    static Assembly compile(string file)
    {
        var syntaxTrees = new SyntaxTree[]{
            CSharpSyntaxTree.ParseText(File.ReadAllText(file))
        };

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
        watcher = new FileSystemWatcher();
        watcher.Filter = "*.cs";
        watcher.Changed += (sender, e) =>
            updateInfos(e.FullPath);
    }
}