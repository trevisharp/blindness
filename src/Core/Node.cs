/* Author:  Leonardo Trevisan Silio
 * Date:    12/08/2024
 */
using System;
using System.Linq;
using System.Threading;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace Blindness.Core;

using Bind;
using Injection;
using Concurrency;

using Core.Injections;

/// <summary>
/// A base class for all concrete nodes.
/// Used by code generator.
/// </summary>
public abstract class Node
{
    readonly static DepFunction depFunction = new DepsDepFunction();
    readonly static BaseTypeFilter filter = new ConcreteFilter();

    public static Node New(Type type)
    {
        var node = DependencySystem.Shared.Get(type,
            depFunction, [ filter ]
        );

        return node as Node;
    }

    Binding Bind;

    
}