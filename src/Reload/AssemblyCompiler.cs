/* Author:  Leonardo Trevisan Silio
 * Date:    12/08/2024
 */
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Runtime.Loader;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

using Blindness.Generators;

namespace Blindness.Reload;

/// <summary>
/// Compile and get a Assembly.
/// </summary>
public class AssemblyCompiler
{
    AssemblyLoadContext lastContext = null;

    public string MainDirectory { get; set; } = Environment.CurrentDirectory;
    public List<Assembly> ExtraReferences { get; private set; } = [];

    /// <summary>
    /// Preprocessor symbols used on compilation.
    /// </summary>
    public List<string> PreprocessorSymbols { get; private set; } = [ "DEBUG", "TRACE" ];

    /// <summary>
    /// Get a new assembly of compilation from files in 'MainDirectory'
    /// </summary>
    public virtual Assembly Get()
    {
        var newAssembly = GetNewAssembly(
            MainDirectory,
            ExtraReferences
        );
        return newAssembly;
    }

    protected virtual IEnumerable<string> FindAllCSharpFiles(
        string directory)
    {
        var files = Directory.GetFiles(
            directory, "*.cs",
            SearchOption.AllDirectories
        );

        var codeFiles =
            from file in files
            where !IsBuildOutput(directory, file)
            where !IsLegacyGeneratedFile(directory, file)
            select file;

        foreach (var file in codeFiles.Distinct())
            yield return file;

        var globalUsings = files
            .Where(f => f.EndsWith(".GlobalUsings.g.cs", StringComparison.OrdinalIgnoreCase))
            .Where(f => IsBuildOutput(directory, f))
            .OrderByDescending(File.GetLastWriteTimeUtc)
            .FirstOrDefault();
        if (globalUsings is not null)
            yield return globalUsings;
    }

    /// <summary>
    /// Returns true if the file is inside a bin or obj directory.
    /// </summary>
    protected static bool IsBuildOutput(string directory, string file)
    {
        var relative = Path.GetRelativePath(directory, file);
        var parts = relative.Split(
            [ Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar ],
            StringSplitOptions.RemoveEmptyEntries
        );
        return parts.Any(p => p.Equals("bin", StringComparison.OrdinalIgnoreCase)
            || p.Equals("obj", StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Returns true if the file was generated on runtime by versions before the source generator.
    /// </summary>
    protected static bool IsLegacyGeneratedFile(string directory, string file)
        => Path.GetRelativePath(directory, file)
            .Split([ Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar ])
            .Contains("NonConstantGeneratedFiles", StringComparer.OrdinalIgnoreCase);

    protected virtual IEnumerable<MetadataReference> GetReferences(
        IEnumerable<Assembly> extraRefs)
    {
        var trusted = (AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES") as string ?? "")
            .Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries);
        
        var loaded = AppDomain.CurrentDomain
            .GetAssemblies()
            .Append(Assembly.GetEntryAssembly())
            .Concat(extraRefs)
            .Where(a => a is not null && !a.IsDynamic && !string.IsNullOrEmpty(a.Location))
            .Select(a => a.Location);
        
        return trusted
            .Concat(loaded)
            .DistinctBy(path => Path.GetFullPath(path), StringComparer.OrdinalIgnoreCase)
            .Where(IsManagedAssembly)
            .Select(path => MetadataReference.CreateFromFile(path));
    }

    static bool IsManagedAssembly(string path)
    {
        try
        {
            AssemblyName.GetAssemblyName(path);
            return true;
        }
        catch { return false; }
    }

    protected virtual Assembly GetNewAssembly(
        string directory,
        IEnumerable<Assembly> extraRefs)
    {
        var parseOptions = new CSharpParseOptions(
            LanguageVersion.Latest,
            preprocessorSymbols: PreprocessorSymbols
        );

        var files = FindAllCSharpFiles(directory);
        var syntaxTrees = files
            .Select(file => CSharpSyntaxTree.ParseText(
                File.ReadAllText(file), parseOptions, file
            ));

        var compilationOptions = new CSharpCompilationOptions(
            OutputKind.ConsoleApplication,
            nullableContextOptions: NullableContextOptions.Enable
        );

        var compilation = CSharpCompilation.Create(
            "HotReloadAppend",
            syntaxTrees: syntaxTrees,
            references: GetReferences(extraRefs),
            options: compilationOptions
        );

        CSharpGeneratorDriver
            .Create(new ConcreteGenerator())
            .RunGeneratorsAndUpdateCompilation(compilation, out var generatedCompilation, out _);

        using var ms = new MemoryStream();
        var result = generatedCompilation.Emit(ms);

        if (!result.Success)
        {
            foreach (var diagnostic in result.Diagnostics)
                if (diagnostic.Severity == DiagnosticSeverity.Error)
                    Verbose.Error(diagnostic);

            return null;
        }

        ms.Seek(0, SeekOrigin.Begin);
        var context = new AssemblyLoadContext($"HotReload-{Guid.NewGuid():N}", isCollectible: true);
        var assembly = context.LoadFromStream(ms);

        lastContext?.Unload();
        lastContext = context;
        return assembly;
    }
}
