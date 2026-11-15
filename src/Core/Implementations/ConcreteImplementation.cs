/* Author:  Leonardo Trevisan Silio
 * Date:    01/01/2024
 */
using System;
using System.Reflection;
using System.Collections.Generic;

namespace Blindness.Core.Implementations;

using Factory;

/// <summary>
/// Add default usings.
/// </summary>
public class ConcreteImplementation : BaseTypeImplementation
{
    public override void ImplementType(
        ClassBuilder builder,
        string fileName, Type baseInterface,
        List<PropertyInfo> properties,
        List<MethodInfo> methods
    )
    {
        builder
            .SetNamespace(baseInterface.Namespace)
            .SetClassName($"{baseInterface.Name}Concrete")
            .AddBaseType("global::Blindness.Core.Node")
            .AddBaseType(TypeNames.CSharpName(baseInterface))
            .AddAttribute("global::Blindness.Core.Concrete");
    }
}