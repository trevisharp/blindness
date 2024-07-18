/* Author:  Leonardo Trevisan Silio
 * Date:    17/07/2024
 */
using System.Linq;
using System.Collections.Generic;

namespace Blindness.States;

using Internal;
using Blindness;
using Exceptions;

/// <summary>
/// A memory abstraction to provide binding features.
/// </summary>
public class Memory(IMemoryBehaviour behaviour)
{
    private static Memory crr = null;
    public static Memory Current => crr;

    public const int Null = -1;

    readonly Dictionary<int, List<PointerListner>> eventDict = [];

    /// <summary>
    /// Add a event to listen a specific memory address.
    /// </summary>
    public void AddPointerListner(int pointer, EventElement element)
    {
        if (pointer == Null)
            return;

        if (!eventDict.ContainsKey(pointer))
            eventDict.Add(pointer, []);
        var events = eventDict[pointer];
        
        var repeatitionElement = events
            .Find(el => el.EventObject == element);
        
        if (repeatitionElement is not null)
        {
            repeatitionElement.Counter++;
            return;
        }

        events.Add(new PointerListner {
            EventObject = element,
            Counter = 1
        });
    }

    /// <summary>
    /// Remove a specific event from a memory address.
    /// </summary>
    public void RemovePointerListner(int pointer, EventElement element)
    {
        if (pointer == Null)
            return;

        if (!eventDict.TryGetValue(pointer, out var events))
            return;
        
        var repeatitionElement = events
            .Find(el => el.EventObject == element);
        if (repeatitionElement is null)
            return;

        repeatitionElement.Counter--;
        if (repeatitionElement.Counter > 0)
            return;
        
        events.Remove(repeatitionElement);
        if (events.Count == 0)
            eventDict.Remove(pointer);
    }

    /// <summary>
    /// Reset memory with a specific memory implementation
    /// </summary>
    public static void Reset(IMemoryBehaviour behaviour)
        => crr = new(behaviour);

    /// <summary>
    /// Add a object to memory and receive your memory address.
    /// </summary>
    public int Add(object obj)
    {
        if (behaviour is null)
            throw new MemoryBehaviourNotDefined();

        return behaviour.Add(obj);
    }

    /// <summary>
    /// Get an object of type T at a memory address.
    /// </summary>
    public T Get<T>(int pointer)
        => (T)GetObject(pointer);
    
    public int Find(object obj)
    {
        if (behaviour is null)
            throw new MemoryBehaviourNotDefined();
        
        return behaviour.Find(obj);
    }

    /// <summary>
    /// Get an object at a memory address.
    /// </summary>
    public object GetObject(int pointer)
    {
        if (behaviour is null)
            throw new MemoryBehaviourNotDefined();

        return behaviour.Get(pointer);
    }

    /// <summary>
    /// Set an object at a memory address with type T.
    /// </summary>
    public void Set<T>(int pointer, T value)
    {
        if (behaviour is null)
            throw new MemoryBehaviourNotDefined();
        
        behaviour.Set(pointer, value);
        CallEvents(pointer);
    }

    /// <summary>
    /// Reload all nodes using dependency injection system.
    /// </summary>
    public void Reload()
    {
        if (behaviour is null)
            throw new MemoryBehaviourNotDefined();

        behaviour.Reload(obj =>
        {
            if (obj is null)
                return null;
            
            var type = obj.GetType();
            if (!type.Implements("INode"))
                return obj;
            
            var baseType = type.GetInterfaces()
                .FirstOrDefault(i => i.Name != "INode");
            
            var node = obj as Node;
            var nodeCopy = DependencySystem
                .Current.GetConcrete(baseType);

            Current.Set(node.MemoryLocation, nodeCopy);
            nodeCopy.MemoryLocation = node.MemoryLocation;
            nodeCopy.Bind.Copy(node.Bind);
            nodeCopy.Model = node.Model;

            return nodeCopy;
        });
    }

    void CallEvents(int pointer)
    {
        if (!eventDict.TryGetValue(pointer, out var events))
            return;
        
        foreach (var item in events)
            item?.EventObject?.Awake();
    }
}