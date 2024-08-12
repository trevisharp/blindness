/* Author:  Leonardo Trevisan Silio
 * Date:    12/07/2024
 */
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Blindness.Reload;

/// <summary>
/// Compile and get a Assembly.
/// </summary>
public class AssemblyCompiler
{
    public string MainDirectory { get; set; } = Environment.CurrentDirectory;
    public List<Assembly> ExtraReferences { get; private set; } = [];

    /// <summary>
    /// Get a new assembly of compilation from files in 'MainDirectory'
    /// </summary>
    public Assembly Get()
    {
        var newAssembly = GetNewAssembly(
            MainDirectory,
            ExtraReferences
        );
        return newAssembly;
    }

    static IEnumerable<string> FindAllCSharpFiles(
        string directory)
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

    static IEnumerable<MetadataReference> GetReferences(
        IEnumerable<Assembly> extraRefs)
    {
        var assembly = Assembly.GetEntryAssembly();
        var assemblies = assembly
            .GetReferencedAssemblies()
            .Select(Assembly.Load)
            .Append(assembly)
            .Append(Assembly.Load("System.Private.CoreLib"))
            .Concat(extraRefs);
        
        return 
            from a in assemblies
            select a.Location into loc
            select MetadataReference.CreateFromFile(loc);
    }

    static Assembly GetNewAssembly(
        string directory,
        IEnumerable<Assembly> extraRefs)
    {
        var files = FindAllCSharpFiles(directory);
        var syntaxTrees = files
            .Select(File.ReadAllText)
            .Select(text => CSharpSyntaxTree.ParseText(text));

        var compilationOptions = new CSharpCompilationOptions(
            OutputKind.ConsoleApplication
        );
        
        var compilation = CSharpCompilation.Create(
            "HotReloadAppend",
            syntaxTrees: syntaxTrees,
            references: GetReferences(extraRefs),
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
}