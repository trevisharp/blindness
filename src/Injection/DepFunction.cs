/* Author:  Leonardo Trevisan Silio
 * Date:    25/07/2024
 */
using System;

namespace Blindness.Injection;

/// <summary>
/// Represents a type of dependency loading function.
/// </summary>
public abstract class DepFunction
{
    /// <summary>
    /// Call the dependency function and returns the loaded object.
    /// </summary>
    public abstract object Call(
        Type type,
        DependencySystem depSys,
        InjectionArgs args
    );

    public readonly static ConstructorDepFunction Constructor = new();
}