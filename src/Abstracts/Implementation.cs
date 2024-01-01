/* Author:  Leonardo Trevisan Silio
 * Date:    01/01/2024
 */
using System;
using System.Text;
using System.Reflection;
using System.Collections.Generic;

namespace Blindness.Abstracts;

/// <summary>
/// Represents a logic to generate code.
/// </summary>
public abstract class Implementation
{
    public abstract void ImplementType(
        string fileName, Type baseInterface,
        IEnumerable<PropertyInfo> properties,
        IEnumerable<MethodInfo> methods,
        StringBuilder a
    );
}