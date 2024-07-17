/* Author:  Leonardo Trevisan Silio
 * Date:    15/07/2024
 */
using System;
using System.Text;
using System.Collections.Generic;

namespace Blindness.Abstracts;

/// <summary>
/// A builder to generate a CSharp class file.
/// </summary>
public class ClassBuilder
{
    string className = "MyClass";
    readonly List<string> usings = [];
    readonly List<string> baseTypes = [];
    readonly List<string> attributes = [];
    readonly StringBuilder classCode = new();
    string tabInfo = "\t";
    const char tab = '\t';

    /// <summary>
    /// Define the name of generated class.
    /// </summary>
    public ClassBuilder SetClassName(string className)
    {
        this.className = className;
        return this;
    }

    /// <summary>
    /// Add a base type that generated class inherits.
    /// </summary>
    public ClassBuilder AddBaseType(string type)
    {
        if (type is null)
            throw new ArgumentNullException(nameof(type));
        
        if (baseTypes.Contains(type))
            return this;
        
        baseTypes.Add(type);
        return this;
    }

    /// <summary>
    /// Add using import on header of the file.
    /// </summary>
    public ClassBuilder AddUsing(string reference)
    {
        if (reference is null)
            throw new ArgumentNullException(nameof(reference));
        
        if (usings.Contains(reference))
            return this;
        
        usings.Add(reference);
        return this;
    }

    /// <summary>
    /// Add data attribute on the top of the generated class.
    /// </summary>
    public ClassBuilder AddAttribute(string attribute)
    {
        if (attribute is null)
            throw new ArgumentNullException(nameof(attribute));
        
        if (attributes.Contains(attribute))
            return this;
        
        attributes.Add(attribute);
        return this;
    }

    /// <summary>
    /// Add a line of code inside of the generated class.
    /// If code has many lines, tabulation is correctly applied.
    /// </summary>
    public ClassBuilder AddLineCode(string code)
    {
        if (code is null)
            throw new ArgumentNullException(nameof(code));
        
        if (code is null)
            return this;
        
        code = tabInfo + code
            .Replace("\r", "")
            .Replace("\n", "\n" + tabInfo);
        classCode.AppendLine(code);
        return this;
    }

    /// <summary>
    /// Add a public property with optional get and set.
    /// If get and set is null, a auto-implemented property is used.
    /// The get/set parameters need be the complete expression.
    /// </summary>
    public ClassBuilder AddProperty(
        string type, string name, 
        string get = null, string set = null)
    {
        if (get is null && set is null)
            AddLineCode($"public {type} {name} {{ get; set; }}");
        
        AddLineCode($"public {type} {name}");

        AddLineCode("{");
        AddScope();

        if (get is not null)
            AddLineCode(get);
        
        if (set is not null)
            AddLineCode(set);
        
        RemoveScope();
        AddLineCode("}");

        return this;
    }

    /// <summary>
    /// Add tabulation for next lines added inside the generated class.
    /// </summary>
    public ClassBuilder AddScope()
    {
        tabInfo += tab;
        return this;
    }

    /// <summary>
    /// Remove tabulation for next lines added inside the generated class.
    /// </summary>
    public ClassBuilder RemoveScope()
    {
        if (tabInfo.Length == 0)
            return this;
        
        tabInfo = tabInfo[1..];
        return this;
    }

    /// <summary>
    /// Build and get the C# code.
    /// </summary>
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
        basesCode.Append(baseTypes[^1]);

        StringBuilder attributeCode = new();
        foreach (var attribute in attributes)
            attributeCode.AppendLine($"[{attribute}]");

        return
        $$"""
        //------------------------------------------------------------------------------
        // <auto-generated>
        //     This code was generated by a tool.
        //
        //     Changes to this file may cause incorrect behavior and will be lost if
        //     the code is regenerated.
        // </auto-generated>
        //------------------------------------------------------------------------------

        {{usingsCode}}
        {{attributeCode}}public partial class {{className}}{{basesCode}}
        {
        {{classCode}}
        }
        """;
    }
}