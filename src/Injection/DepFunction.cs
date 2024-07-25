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
        Func<Type, TypeList, DepFunction, object> depSys,
        TypeList deepDeps
    );

    public readonly static ConstructorDepFunction Constructor = new();
}