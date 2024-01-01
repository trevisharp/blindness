/* Author:  Leonardo Trevisan Silio
 * Date:    01/01/2024
 */
using System;
using System.Reflection;
using System.Collections.Generic;

namespace Blindness.Abstracts.Implementations;

/// <summary>
/// Generate properties by default behavior.
/// </summary>
public class DefaultPropertyImplementation : Implementation
{
    public override void ImplementType(
        ClassBuilder builder,
        string fileName, Type baseInterface,
        List<PropertyInfo> properties,
        List<MethodInfo> methods
    )
    {
        for (int i = 0; i < properties.Count; i++)
        {
            var prop = properties[i];
            var typeName = ArrangeTypeName(prop.PropertyType);
            builder
                .AddLineCode($"public {typeName} {prop.Name}")
                .AddLineCode("{")
                .AddScope()
                .AddLineCode($"get => Bind.Get<{typeName}>({i});")
                .AddLineCode($"set => Bind.Set({i}, value);")
                .RemoveScope()
                .AddLineCode("}");
            
            properties.Remove(prop);
        }
    }
}