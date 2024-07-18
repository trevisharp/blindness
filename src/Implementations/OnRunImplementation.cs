/* Author:  Leonardo Trevisan Silio
 * Date:    01/01/2024
 */
using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace Blindness.Factory.Implementations;

/// <summary>
/// Generate OnRun Function code.
/// </summary>
public class OnRunImplementation : Implementation
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
        
        builder.AddLineCode(
            $$"""
            protected override void OnRun()
                => (({{baseInterface.Name}})this).OnRun();
            """
        );
        methods.Remove(onRun);
    }
}