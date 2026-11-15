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
        foreach (var prop in properties)
        {
            var typeName = TypeNames.CSharpName(prop.PropertyType);
            builder
                .CreateProperty()
                    .AddAttribute("global::Blindness.Bind.Binding")
                    .SetType(typeName)
                    .SetName(prop.Name)
                    .SetGetCode($"global::Blindness.Bind.Binding.Get(this).Open<{typeName}>(nameof({prop.Name}))")
                    .SetSetCode($"global::Blindness.Bind.Binding.Get(this).Place(nameof({prop.Name}), value)")
                .AppendMember();
        }
        properties.Clear();
    }
}