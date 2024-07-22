/* Author:  Leonardo Trevisan Silio
 * Date:    01/01/2024
 */
using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace Blindness.Core.Implementations;

using Factory;

/// <summary>
/// Generate Deps Function code.
/// </summary>
public class DepsImplementation : BaseTypeImplementation
{
    public override void ImplementType(
        ClassBuilder builder,
        string fileName, Type baseInterface,
        List<PropertyInfo> properties,
        List<MethodInfo> methods
    )
    {
        var deps = methods
            .FirstOrDefault(m => m.Name == "Deps");
        if (deps is null)
            return;
        
        builder.AddCodeLine("public void Deps(");

        var parameters = deps.GetParameters();
        builder.AddScope();
        for (int i = 0; i < parameters.Length; i++)
        {
            var parameter = parameters[i];
            builder.AddCodeLine(
                i < parameters.Length - 1 ?
                $"{parameter.ParameterType.Name} {parameter.Name}," :
                $"{parameter.ParameterType.Name} {parameter.Name}"
            );
        }
        builder
            .RemoveScope()
            .AddCodeLine(")")
            .AddCodeLine("{")
            .AddScope();
        
        for (int i = 0; i < parameters.Length; i++)
        {
            var parameter = parameters[i];
            builder.AddCodeLine(
                $"this.{parameter.Name} = {parameter.Name};"
            );
        }
        builder
            .RemoveScope()
            .AddCodeLine("}");

        methods.Remove(deps);
    }
}