/* Author:  Leonardo Trevisan Silio
 * Date:    01/01/2024
 */
using System;
using System.Reflection;
using System.Collections.Generic;

namespace Blindness.Core.Implementations;

using Factory;

/// <summary>
/// Generate constructor with binding by default behavior.
/// </summary>
public class ConstructorImplementation : BaseTypeImplementation
{
    public override void ImplementType(
        ClassBuilder builder,
        string fileName, Type baseInterface,
        List<PropertyInfo> properties,
        List<MethodInfo> methods
    )
    {
        builder
            .AddCodeLine($"public {baseInterface.Name}Concrete()")
            .AddScope()
            .AddCodeLine("=> this.Bind = new Binding(")
            .AddScope()
            .AddCodeLine($"this, {properties.Count}, typeof({baseInterface.Name}),")
            .AddCodeLine("s => s switch")
            .AddCodeLine("{")
            .AddScope();

        for (int i = 0; i < properties.Count; i++)
        {
            var prop = properties[i];
            builder.AddCodeLine(
                $"\"{prop.Name}\" => {i},"
            );
        }
        builder
            .AddCodeLine("_ => -1")
            .RemoveScope()
            .AddCodeLine("}")
            .RemoveScope()
            .AddCodeLine(");")
            .RemoveScope();
    }
}