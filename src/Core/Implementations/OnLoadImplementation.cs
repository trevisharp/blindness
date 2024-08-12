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
/// Generate OnLoad Function code.
/// </summary>
public class OnLoadImplementation : BaseTypeImplementation
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
        
        builder.AddCodeLine(
            $$"""
            public override void Load()
                => (({{baseInterface.Name}})this).OnLoad();
            """
        );
        methods.Remove(onLoad);
        
    }
}