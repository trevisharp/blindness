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
        ) as Node;
        node?.Load();

        return node;
    }

    Binding internalBind = new();

    public static Node Replace(Type type, Node oldNode)
    {
        throw new NotImplementedException();
    }

    public virtual void Run() { }
    public virtual void Load() { }

    public void Bind(Expression<Func<bool>> binding)
        => Binding.Bind(binding);

    public void When(Func<bool> condition, Action action)
    {
        throw new NotImplementedException();
    }

    public void On(Expression<Func<bool>> condition, Action<bool> action)
    {
        throw new NotImplementedException();
    }
}