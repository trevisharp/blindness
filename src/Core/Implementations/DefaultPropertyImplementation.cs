/* Author:  Leonardo Trevisan Silio
 * Date:    01/01/2024
 */
using System;
using System.Reflection;
using System.Collections.Generic;

namespace Blindness.Core.Implementations;

using Factory;

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
        for (int i = 0; properties.Count > 0; i++)
        {
            var prop = properties[0];
            var typeName = ArrangeGenericTypeName(prop.PropertyType);
            builder
                .AddProperty(typeName, prop.Name, 
                    $"get => Bind.Get<{typeName}>({i});",
                    $"set => Bind.Set({i}, value);"
                );
            
            properties.Remove(prop);
        }
    }
}