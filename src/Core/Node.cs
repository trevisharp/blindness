/* Author:  Leonardo Trevisan Silio
 * Date:    14/08/2024
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
        var obj = DependencySystem.Shared.Get(type,
            depFunction, [ filter ]
        );

        if (obj is not Node node)
            throw new InvalidOperationException($"The {type} not inherits from Node type.");
        return node;
    }

    public static Node Recreate(Assembly assembly, Node oldNode)
    {
        ArgumentNullException.ThrowIfNull(oldNode, nameof(oldNode));
        ArgumentNullException.ThrowIfNull(assembly, nameof(assembly));

        var oldType = oldNode.GetType();
        var okTypes = 
            from t in assembly.GetTypes()
            where t.Name == oldType.Name
            select t;
        
        if (okTypes.Count() != 1)
            throw new Exception("1");
        
        var newType = okTypes.First();
        var newNode = New(newType);
        CopyNonNodeValues(oldNode, newNode);
        
        return newNode;
    }

    public static void CopyNonNodeValues(Node source, Node target)
    {
        var targetType = target.GetType();
        foreach (var prop in source.GetType().GetProperties())
        {
            var targetProp = targetType.GetProperty(prop.Name);
            if (targetProp is null)
                continue;
            
            if (prop.PropertyType != targetProp.PropertyType)
                continue;
            
            if (prop.PropertyType.Implements(typeof(INode)))
                continue;
            
            var value = prop.GetValue(source);
            targetProp.SetData(target, value);
        }
    }

    Binding internalBind = new();
    readonly List<OnEvent> ons = [];
    readonly List<WhenEvent> whens = [];
    class OnEvent
    {
        public OnEvent(Func<bool> trigger, Action<bool> action)
        {
            this.action = action;
            this.trigger = trigger;
            last = trigger();
            action(last);
        }

        public Action<bool> action;
        public Func<bool> trigger;
        public bool last = false;
    }
    class WhenEvent(Func<bool> trigger, Action action)
    {
        public Action action = action;
        public Func<bool> trigger = trigger;
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

    /// <summary>
    /// Bind two expressions with Bind(myField == expression).
    /// </summary>
    public void Bind(Expression<Func<bool>> binding)
        => Binding.Bind(binding);

    /// <summary>
    /// Add a event that already call a action when the trigger is true.
    /// </summary>
    public void When(Func<bool> trigger, Action action)
    {
        ArgumentNullException.ThrowIfNull(action, nameof(action));
        ArgumentNullException.ThrowIfNull(trigger, nameof(trigger));

        whens.Add(new(trigger, action));
    }

    /// <summary>
    /// Add a event that already call a action when the trigger values change.
    /// </summary>
    public void On(Func<bool> trigger, Action<bool> action)
    {
        ArgumentNullException.ThrowIfNull(action, nameof(action));
        ArgumentNullException.ThrowIfNull(trigger, nameof(trigger));
        
        ons.Add(new(trigger, action));
    }
}