/* Author:  Leonardo Trevisan Silio
 * Date:    01/01/2024
 */
using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace Blindness.Abstracts.Implementations;

/// <summary>
/// Generate Deps Function code.
/// </summary>
public class DepsImplementation : Implementation
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
        
        builder.AddLineCode("public void Deps(");

        var parameters = deps.GetParameters();
        builder.AddScope();
        for (int i = 0; i < parameters.Length; i++)
        {
            var parameter = parameters[i];
            builder.AddLineCode(
                i < parameters.Length - 1 ?
                $"{parameter.ParameterType.Name} {parameter.Name}," :
                $"{parameter.ParameterType.Name} {parameter.Name}"
            );
        }
        builder
            .RemoveScope()
            .AddLineCode(")")
            .AddLineCode("{")
            .AddScope();
        
        for (int i = 0; i < parameters.Length; i++)
        {
            var parameter = parameters[i];
            builder.AddLineCode(
                $"this.{parameter.Name} = {parameter.Name};"
            );
        }
        builder
            .RemoveScope()
            .AddLineCode("}");

        methods.Remove(deps);
    }
}