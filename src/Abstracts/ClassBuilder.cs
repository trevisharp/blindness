/* Author:  Leonardo Trevisan Silio
 * Date:    01/01/2024
 */
using System.Text;
using System.Collections.Generic;

namespace Blindness.Abstracts;

/// <summary>
/// A builder to generate a CSharp class file.
/// </summary>
public class ClassBuilder
{
    string className = "MyClass";
    List<string> usings = new();
    List<string> baseTypes = new();
    List<string> attributes = new();
    StringBuilder classCode = new();
    string tabInfo = "\t";
    const char tab = '\t';

    public ClassBuilder SetClassName(string className)
    {
        this.className = className;
        return this;
    }

    public ClassBuilder AddBaseType(string type)
    {
        if (baseTypes.Contains(type))
            return this;
        
        baseTypes.Add(type);
        return this;
    }

    public ClassBuilder AddUsing(string reference)
    {
        if (usings.Contains(reference))
            return this;
        
        usings.Add(reference);
        return this;
    }

    public ClassBuilder AddAttribute(string attribute)
    {
        if (attributes.Contains(attribute))
            return this;
        
        attributes.Add(attribute);
        return this;
    }

    public ClassBuilder AddLineCode(string code)
    {
        if (code is null)
            return this;
        
        code = code.Replace("\r", "")
            .Replace("\n", "\n" + tabInfo);
        classCode.AppendLine(code);
        return this;
    }

    public ClassBuilder AddScope()
    {
        tabInfo += tab;
        return this;
    }

    public ClassBuilder RemoveScope()
    {
        if (tabInfo.Length == 0)
            return this;
        
        tabInfo = tabInfo.Remove(0);
        return this;
    }

    public string Build()
    {
        StringBuilder usingsCode = new();
        foreach (var reference in usings)
            usingsCode.AppendLine($"using {reference};");
        
        StringBuilder basesCode = new();
        if (baseTypes.Count > 0)
            basesCode.Append(" : ");
        for (int i = 0; i < baseTypes.Count - 1; i++)
        {
            basesCode.Append(baseTypes[i]);
            basesCode.Append(", ");
        }
        basesCode.Append(basesCode[^1]);

        StringBuilder attributeCode = new();
        foreach (var attribute in attributes)
            attributeCode.AppendLine($"[{attribute}]");

        return
        $$"""
        {{usingsCode}}

        {{attributeCode}}
        public class {{className}}{{basesCode}}
        {
        {{classCode}}
        }
        """;
    }
}