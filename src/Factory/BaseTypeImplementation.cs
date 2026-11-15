/* Author:  Leonardo Trevisan Silio
 * Date:    11/07/2024
 */
using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace Blindness.Factory;

/// <summary>
/// Represents a logic to generate code for implement a base type.
/// </summary>
public abstract class BaseTypeImplementation
{
    public abstract void ImplementType(
        ClassBuilder builder,
        string fileName, 
        Type implementedType,
        List<PropertyInfo> properties,
        List<MethodInfo> methods
    );
    
    /// <summary>
    /// Get a fully qualified C# name for a type, usable in generated code.
    /// </summary>
    protected static string ArrangeGenericTypeName(Type type)
        => TypeNames.CSharpName(type);
}