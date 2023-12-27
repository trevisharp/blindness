using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Blindness.Abstracts;

public static class CSharpCompiler
{
    public static void Recompile()
    {
        string csprojPath = Directory
            .GetFiles(Environment.CurrentDirectory)
            .FirstOrDefault(f => f.EndsWith(".csproj"));

        Assembly assembly = CompileProjet(csprojPath);
        assembly.EntryPoint.Invoke(null, new object[] {
            new string[0]
        });
    }

    static Assembly CompileProjet(string csprojPath)
    {
        var sourceFiles = findCSharpFiles(Environment.CurrentDirectory);

        var syntaxTrees = sourceFiles
            .Select(file => CSharpSyntaxTree.ParseText(File.ReadAllText(file)));

        var compilationOptions = new CSharpCompilationOptions(
            OutputKind.ConsoleApplication
        );
        var assembly = Assembly.GetEntryAssembly();
        var references = assembly
            .GetReferencedAssemblies()
            .Select(r => Assembly.Load(r))
            .Append(Assembly.GetEntryAssembly())
            .Append(Assembly.Load("System.Private.CoreLib"))
            .Append(Assembly.Load("System.Linq.Expressions"))
            .Select(r => MetadataReference.CreateFromFile(r.Location));
        
        foreach (var reference in references)
            Verbose.Success(reference.FilePath);
        
        var compilation = CSharpCompilation.Create(
            "HotReload",
            syntaxTrees: syntaxTrees,
            references: references,
            options: compilationOptions
        );

        using (MemoryStream ms = new MemoryStream())
        {
            var resultado = compilation.Emit(ms);

            if (resultado.Success)
            {
                ms.Seek(0, SeekOrigin.Begin);
                return Assembly.Load(ms.ToArray());
            }
            else
            {
                foreach (var diagnostic in resultado.Diagnostics)
                    Verbose.Error(diagnostic);
                return null;
            }
        }
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
