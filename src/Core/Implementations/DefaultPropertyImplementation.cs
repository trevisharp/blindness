/* Author:  Leonardo Trevisan Silio
 * Date:    12/08/2024
 */
using System;
using System.Reflection;
using System.Collections.Generic;

namespace Blindness.Core.Implementations;

using Factory;

/// <summary>
/// Generate properties by default behavior.
/// </summary>
public class DefaultPropertyImplementation : BaseTypeImplementation
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
                .CreateProperty()
                    .AddAttribute("Binding")
                    .SetType(typeName)
                    .SetName(prop.Name)
                    .SetGetCode($"Binding.Get(this).Open<{typeName}>(nameof({prop.Name}))")
                    .SetSetCode($"Binding.Get(this).Place(nameof({prop.Name}), value)")
                .AppendMember();
            properties.Remove(prop);
        }
    }
}