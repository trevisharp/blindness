/* Author:  Leonardo Trevisan Silio
 * Date:    25/07/2024
 */
using System;
using System.Collections.Generic;

namespace Blindness.Injection;

/// <summary>
/// Represents the args of a dependency call invokation.
/// </summary>
public class InjectionArgs(DepFunction function)
{
    public readonly static InjectionArgs Default
        = new(DepFunction.Constructor);
    
    public DepFunction DepFunction { get; set; } = function;
    public Stack<Type> DependencyGraph { get; set; } = [];
}