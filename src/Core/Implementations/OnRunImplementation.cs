/* Author:  Leonardo Trevisan Silio
 * Date:    12/08/2024
 */
using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace Blindness.Core.Implementations;

using Factory;

/// <summary>
/// Generate OnRun Function code.
/// </summary>
public class OnRunImplementation : BaseTypeImplementation
{
    public override void ImplementType(
        ClassBuilder builder,
        string fileName, Type baseInterface,
        List<PropertyInfo> properties,
        List<MethodInfo> methods
    )
    {
        var onRun = methods
            .FirstOrDefault(m => m.Name == "OnRun");
        if (onRun is null)
            return;
        
        builder.AddCodeLine(
            $$"""
            public override void Run()
                => (({{baseInterface.Name}})this).OnRun();
            """
        );
        methods.Remove(onRun);
    }
}