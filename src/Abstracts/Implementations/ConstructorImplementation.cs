/* Author:  Leonardo Trevisan Silio
 * Date:    01/01/2024
 */
using System;
using System.Reflection;
using System.Collections.Generic;

namespace Blindness.Abstracts.Implementations;

/// <summary>
/// Generate constructor with binding by default behavior.
/// </summary>
public class ConstructorImplementation : Implementation
{
    public override void ImplementType(
        ClassBuilder builder,
        string fileName, Type baseInterface,
        List<PropertyInfo> properties,
        List<MethodInfo> methods
    )
    {
        builder
            .AddLineCode($"public {baseInterface.Name}Concrete()")
            .AddScope()
            .AddLineCode("=> this.Bind = new Binding(")
            .AddScope()
            .AddLineCode($"this, {properties.Count}, typeof({baseInterface.Name}),")
            .AddLineCode("s => s switch")
            .AddLineCode("{")
            .AddScope();

        for (int i = 0; i < properties.Count; i++)
        {
            var prop = properties[i];
            builder.AddLineCode(
                $"\"{prop.Name}\" => {i},"
            );
        }
        builder
            .AddLineCode("_ => -1")
            .RemoveScope()
            .AddLineCode("}")
            .RemoveScope()
            .AddLineCode(");")
            .RemoveScope();
    }
}