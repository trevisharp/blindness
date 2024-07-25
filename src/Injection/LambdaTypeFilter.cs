/* Author:  Leonardo Trevisan Silio
 * Date:    23/07/2024
 */
using System;

namespace Blindness.Injection;

/// <summary>
/// Represents a Filter of types.
/// </summary>
public class LambdaTypeFilter(Predicate<Type> predicate) : BaseTypeFilter
{
    public override bool Filter(Type type)
        => predicate(type);
}