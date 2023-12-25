using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

var dir = Environment.CurrentDirectory;
var csFiles = findCSharpFiles(dir);
var interfaces = findAllInterfaces(csFiles);
var nodes = getNodes(interfaces);

System.Console.WriteLine("Generatig...");
foreach (var node in nodes)
    System.Console.WriteLine(node);

Interface[] getNodes(IEnumerable<Interface> interfaces)
{
    var interfaceList = interfaces.ToList();
    List<string> nodeInterfaces = new List<string> {
        "INode"
    };
    List<Interface> result = new List<Interface>();

    bool needContinue = true;
    while (needContinue)
    {
        needContinue = false;
        for (int i = 0; i < interfaceList.Count; i++)
        {
            var el = interfaceList[i];
            if (!nodeInterfaces.Contains(el.Base))
                continue;

            i--;
            result.Add(el);
            needContinue = true;
            interfaceList.Remove(el);
            nodeInterfaces.Add(el.Name);
        }
    }

    return result.ToArray();
}

IEnumerable<Interface> findAllInterfaces(IEnumerable<string> files)
{
    foreach (var file in files)
    {
        foreach (var element in findInterfaces(file))
            yield return element;
    }
}

IEnumerable<Interface> findInterfaces(string file)
{
    var code = File.ReadAllText(file);
    var terms = code.Split('\t', ' ', '\n');
    
    Interface element = null;
    foreach (var term in terms)
    {
        var data = term.Trim();
        if (string.IsNullOrEmpty(data))
            continue;
        
        if (data == "interface")
        {
            element = new Interface();
            element.File = file;
            continue;
        }

        if (element is null)
            continue;
        
        if (element.Name is null)
        {
            element.Name = data;
            continue;
        }

        if (data == ":")
            continue;
        
        if (element.Base is null)
        {
            element.Base = data;
            yield return element;
            element = null;
        }
    }

    if (element is not null)
        yield return element;
} 

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

public class Interface
{
    public string Name { get; set; }
    public string Base { get; set; }
    public string File { get; set; }

    public override string ToString()
        => $"{Name} : {Base} in {File}";
}