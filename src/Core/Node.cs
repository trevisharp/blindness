/* Author:  Leonardo Trevisan Silio
 * Date:    14/08/2024
 */
using System;
using System.Linq;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace Blindness.Core;

using Bind;
using Injection;

using Core.Injections;
using Blindness.Exceptions;

/// <summary>
/// A base class for all concrete nodes.
/// Used by code generator.
/// </summary>
public abstract class Node
{
    readonly static DepFunction depFunction = new DepsDepFunction();
    readonly static BaseTypeFilter filter = new ConcreteFilter();

    /// <summary>
    /// Uses depedency injection to create a new node.
    /// </summary>
    public static Node New(Type type)
    {
        type = DependencySystem
            .Shared.GetCompatibilityType(type);
        
        var obj = DependencySystem.Shared.Get(type,
            depFunction, [ filter ]
        );

        if (obj is not Node node)
            throw new InvalidOperationException($"The {type} not inherits from Node type.");
        return node;
    }

    /// <summary>
    /// Recreate the Node based on a new assembly and a oldNode.
    /// Recreate all subnodes and apply CopyNonValues function between the old and new nodes.
    /// </summary>
    public static Node Recreate(Assembly assembly, Node oldNode)
    {
        ArgumentNullException.ThrowIfNull(oldNode, nameof(oldNode));
        ArgumentNullException.ThrowIfNull(assembly, nameof(assembly));

        var oldType = oldNode.GetType();
        var okTypes = assembly
            .GetTypes()
            .Where(t => t.FullName == oldType.FullName)
            .ToArray();
        
        if (okTypes.Length != 1)
            throw new InvalidOperationException(
                $"Hot reload expected one type named {oldType.FullName} in the new assembly, found {okTypes.Length}."
            );
        
        var newType = okTypes[0];
        var newNode = New(newType);
        CopyNonNodeValues(oldNode, newNode);
        
        return newNode;
    }

    /// <summary>
    /// Copy all non Node properties from source to target based on name and type.
    /// If a source has a inner Node property A and target has a inner Node Property B,
    /// CopyNonNodeValues(A, B) is automatically applied.
    /// Ignore copies when the property is a readonly binding binding.
    /// </summary>
    public static void CopyNonNodeValues(Node source, Node target)
    {
        if (source is null || target is null)
            return;

        var targetType = target.GetType();
        foreach (var prop in source.GetType().GetProperties())
        {
            var targetProp = targetType.GetProperty(prop.Name);
            if (targetProp is null)
                continue;
            
            if (prop.PropertyType != targetProp.PropertyType)
                continue;
            
            if (!prop.PropertyType.Implements(typeof(INode)))
            {
                var value = prop.GetValue(source);
                TrySet(value, targetProp);
                continue;
            }
            
            var srcNode = prop.GetValue(source) as Node;
            var tgtNode = prop.GetValue(target) as Node;
            CopyNonNodeValues(srcNode, tgtNode);
        }

        void TrySet(object value, PropertyInfo targetProp)
        {
            // Try set a property that can contains a ReadonlyBox
            try { targetProp.SetData(target, value); }
            // Ignore readonly boxes copies
            catch (ReadonlyBoxException) { }
            catch (Exception ex) when (ex.InnerException is ReadonlyBoxException) { }
        }
    }

    const int maxEventRounds = 100;

    /// <summary>
    /// Occurs when a On event of a node needs to run on the next EvaluateEvents.
    /// </summary>
    public static event Action OnEventsPending;

    Binding internalBind = new();
    readonly List<OnEvent> ons = [];
    readonly List<WhenEvent> whens = [];
    readonly List<OnEvent> pendingOns = [];
    readonly object pendingSync = new();

    /// <summary>
    /// Represents a On event. It runs again only when a box read by the trigger
    /// changes, or on every evaluation when the trigger reads no box.
    /// </summary>
    class OnEvent
    {
        public OnEvent(Node node, Func<bool> trigger, Action<bool> action)
        {
            this.node = node;
            this.action = action;
            this.trigger = trigger;
            last = Evaluate();
            action(last);
        }

        readonly Node node;
        HashSet<INotifyBox> dependencies = [];
        public Action<bool> action;
        public Func<bool> trigger;
        public bool last = false;
        public bool pending = false;

        public bool IsPolling => dependencies.Count == 0;

        public void Run()
        {
            var result = Evaluate();
            if (result == last)
                return;

            last = result;
            action(result);
        }

        bool Evaluate()
        {
            HashSet<INotifyBox> reads = [];
            var result = BoxTracker.Track(trigger, reads);

            foreach (var box in dependencies)
            {
                if (!reads.Contains(box))
                    box.Changed -= MarkPending;
            }

            foreach (var box in reads)
            {
                if (!dependencies.Contains(box))
                    box.Changed += MarkPending;
            }

            dependencies = reads;
            return result;
        }

        void MarkPending()
            => node.MarkPending(this);
    }
    class WhenEvent
    {
        public WhenEvent(Func<bool> trigger, Action action)
        {
            this.action = action;
            this.trigger = trigger;
            if (trigger())
                action();
        }
        public Action action;
        public Func<bool> trigger;
    }

    public virtual void Run() { }
    public virtual void Load() { }

    public void EvaluateEvents()
    {
        var onCount = ons.Count;
        for (int i = 0; i < onCount; i++)
        {
            var on = ons[i];
            if (on.IsPolling)
                on.Run();
        }

        RunPendingOns();

        var whenCount = whens.Count;
        for (int i = 0; i < whenCount; i++)
        {
            var when = whens[i];
            if (when.trigger())
                when.action();
        }
    }

    /// <summary>
    /// Run all marked On events, including the ones marked by these runs.
    /// </summary>
    void RunPendingOns()
    {
        for (int round = 0; round < maxEventRounds; round++)
        {
            OnEvent[] batch;
            lock (pendingSync)
            {
                if (pendingOns.Count == 0)
                    return;

                batch = [ ..pendingOns ];
                pendingOns.Clear();
                foreach (var on in batch)
                    on.pending = false;
            }

            foreach (var on in batch)
                on.Run();
        }

        Verbose.Warning($"On events of {GetType().Name} still changing after {maxEventRounds} rounds; the remaining run on the next evaluation.", 1);
    }

    /// <summary>
    /// Mark a On event to run on the next evaluation.
    /// </summary>
    void MarkPending(OnEvent on)
    {
        bool first;
        lock (pendingSync)
        {
            if (on.pending)
                return;

            on.pending = true;
            first = pendingOns.Count == 0;
            pendingOns.Add(on);
        }

        if (first)
            OnEventsPending?.Invoke();
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
    /// The trigger runs again only when a property it reads changes.
    /// </summary>
    public void On(Func<bool> trigger, Action<bool> action)
    {
        ArgumentNullException.ThrowIfNull(action, nameof(action));
        ArgumentNullException.ThrowIfNull(trigger, nameof(trigger));

        ons.Add(new(this, trigger, action));
    }
}