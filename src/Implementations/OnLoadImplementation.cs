/* Author:  Leonardo Trevisan Silio
 * Date:    01/01/2024
 */
using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace Blindness.Factory.Implementations;

/// <summary>
/// Generate OnLoad Function code.
/// </summary>
public class OnLoadImplementation : Implementation
{
    public override void ImplementType(
        ClassBuilder builder,
        string fileName, Type baseInterface,
        List<PropertyInfo> properties,
        List<MethodInfo> methods
    )
    {
        var onLoad = methods
            .FirstOrDefault(m => m.Name == "OnLoad");
        if (onLoad is null)
            return;
        
        builder.AddLineCode(
            $$"""
            protected override void OnLoad()
                => (({{baseInterface.Name}})this).OnLoad();
            """
        );
        methods.Remove(onLoad);
        
    }
}