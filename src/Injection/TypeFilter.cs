/* Author:  Leonardo Trevisan Silio
 * Date:    24/07/2024
 */
using System;
using System.Reflection;

namespace Blindness.Injection;

/// <summary>
/// Represents a Filter of types.
/// </summary>
public abstract class TypeFilter
{
    public abstract bool Filter(Type type);

    public static TypeFilter All
        => new LambdaTypeFilter(t => true);

    public static TypeFilter ByAttribute(Type attributeType)
    {
        ArgumentNullException.ThrowIfNull(attributeType, nameof(attributeType));
        LambdaTypeFilter filter = new (t => t.GetCustomAttribute(attributeType) is not null);
        return filter;
    }

    public static TypeFilter ByBaseType(Type baseType)
    {
        ArgumentNullException.ThrowIfNull(baseType, nameof(baseType));
        LambdaTypeFilter filter = new (t => t.BaseType == baseType);
        return filter;
    }

    public static TypeFilter ByImplements(Type baseType)
    {
        ArgumentNullException.ThrowIfNull(baseType, nameof(baseType));
        LambdaTypeFilter filter = new (t => t.Implements(baseType));
        return filter;
    }
}