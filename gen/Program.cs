using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

var dir = Environment.CurrentDirectory;
var csFiles = findCSharpFiles(dir);

System.Console.WriteLine("Generating...");
foreach (var file in csFiles)
    System.Console.WriteLine(file);

IEnumerable<string> findCSharpFiles(string directory)
{
    var files =
        Directory.GetFiles(directory)
        .Where(file => file.EndsWith(".cs"))
        .Where(file => !file.EndsWith(".g.cs"));
    
    foreach (var file in files)
        yield return file;
    
    var directories = Directory.GetDirectories(directory);

    foreach (var dir in directories)
    {
        files = findCSharpFiles(dir);
        foreach (var file in files)
            yield return file;
    }
}

public class Cache
{
    public List<string> Interfaces { get; private set; } = new();
}

public class Interface
{
    public string Name { get; set; }
    public string Base { get; set; }
    public string File { get; set; }
    public int Line { get; set; }
}