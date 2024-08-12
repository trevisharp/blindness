/* Author:  Leonardo Trevisan Silio
 * Date:    12/08/2024
 */
using System;
using System.Reflection;

namespace Blindness.Core.Injections;

using Injection;

public class ConcreteFilter : BaseTypeFilter
{
    public override bool Filter(Type type)
    {
        ArgumentNullException.ThrowIfNull(type, nameof(type));
        return type.GetCustomAttribute(typeof(ConcreteAttribute)) is not null;
    }
}
