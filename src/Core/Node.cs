/* Author:  Leonardo Trevisan Silio
 * Date:    13/08/2024
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
    readonly List<OnEvent> ons = [];
    readonly List<WhenEvent> whens = [];
    class OnEvent(Func<bool> trigger, Action<bool> action)
    {
        public Action<bool> action = action;
        public Func<bool> trigger = trigger;
        public bool last = trigger();
    }
    class WhenEvent(Func<bool> trigger, Action action)
    {
        public Action action = action;
        public Func<bool> trigger = trigger;
    }

    public static Node Replace(Type type, Node oldNode)
    {
        throw new NotImplementedException();
    }

    public virtual void Run() { }
    public virtual void Load() { }

    public void EvaluateEvents()
    {
        foreach (var on in ons)
        {
            var result = on.trigger();
            if (result == on.last)
                continue;
            
            on.last = result;
            on.action(result);
        }

        foreach (var when in whens)
        {
            if (when.trigger())
                when.action();
        }
    }

    public void Bind(Expression<Func<bool>> binding)
        => Binding.Bind(binding);

    public void When(Func<bool> condition, Action action)
    {
        ArgumentNullException.ThrowIfNull(action, nameof(action));
        ArgumentNullException.ThrowIfNull(condition, nameof(condition));

        whens.Add(new(condition, action));
    }

    public void On(Func<bool> condition, Action<bool> action)
    {
        ArgumentNullException.ThrowIfNull(action, nameof(action));
        ArgumentNullException.ThrowIfNull(condition, nameof(condition));

        ons.Add(new(condition, action));
    }
}