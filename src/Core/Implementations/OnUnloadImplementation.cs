/* Author:  Leonardo Trevisan Silio
 * Date:    16/11/2026
 */
using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace Blindness.Core.Implementations;

using Factory;

/// <summary>
/// Generate OnUnload Function code.
/// </summary>
public class OnUnloadImplementation : BaseTypeImplementation
{
    public override void ImplementType(
        ClassBuilder builder,
        string fileName, Type baseInterface,
        List<PropertyInfo> properties,
        List<MethodInfo> methods
    )
    {
        var onUnload = methods
            .FirstOrDefault(m => m.Name == "OnUnload");
        if (onUnload is null)
            return;

        builder.AddCodeLine(
            $$"""
            public override void Unload()
            {
                (({{TypeNames.CSharpName(baseInterface)}})this).OnUnload();
                base.Unload();
            }
            """
        );
        methods.Remove(onUnload);
    }
}
