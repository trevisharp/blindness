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
public class DefaultUsingsImplementation : BaseTypeImplementation
{
    public override void ImplementType(
        ClassBuilder builder,
        string fileName, Type baseInterface,
        List<PropertyInfo> properties,
        List<MethodInfo> methods
    )
    {
        builder
            .AddUsing("System")
            .AddUsing("System.Reflection")
            .AddUsing("System.Collections.Generic")
            .AddUsing("Blindness")
            .AddUsing("Blindness.States");
    }
}