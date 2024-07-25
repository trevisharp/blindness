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
    public abstract object Call(Type type, DependencySystem depSys);
}