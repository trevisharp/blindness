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
            var field = $"__{prop.Name}Box";
            var box = $"({field} ??= global::Blindness.Bind.Binding.Get(this).Dictionary.GetOrCreate<{typeName}>(nameof({prop.Name})))";
            builder.AddCodeLine($"global::Blindness.Bind.IBox<{typeName}, {typeName}> {field};");
            builder
                .CreateProperty()
                    .AddAttribute("global::Blindness.Bind.Binding")
                    .SetType(typeName)
                    .SetName(prop.Name)
                    .SetGetCode($"{box}.Open()")
                    .SetSetCode($"{box}.Place(value)")
                .AppendMember();
        }
        properties.Clear();
    }
}