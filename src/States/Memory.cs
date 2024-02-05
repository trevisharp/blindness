/* Author:  Leonardo Trevisan Silio
 * Date:    05/02/2024
 */
using System.Linq;
using System.Collections.Generic;

namespace Blindness.States;

using Internal;
using Exceptions;
using Concurrency.Elements;

/// <summary>
/// A memory abstraction to provide binding features.
/// </summary>
public class Memory
{
    private IMemoryBehaviour behaviour;
    private Memory(IMemoryBehaviour behaviour)
        => this.behaviour = behaviour;
    private static Memory crr = null;
    public static Memory Current => crr;
    
    Dictionary<int, List<PointerListner>> eventDict = new();

    /// <summary>
    /// Add a event to listen a specific memory address.
    /// </summary>
    public void AddPointerListner(int pointer, EventElement element)
    {
        if (!eventDict.ContainsKey(pointer))
            eventDict.Add(pointer, new());
        var events = eventDict[pointer];
        
        var repeatitionElement = events
            .FirstOrDefault(el => el.EventObject == element);
        
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
        if (pointer == -1)
            return;

        if (!eventDict.ContainsKey(pointer))
            return;
        var events = eventDict[pointer];
        
        var repeatitionElement = events
            .FirstOrDefault(el => el.EventObject == element);
        
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
        if (this.behaviour is null)
            throw new MemoryBehaviourNotDefined();

        return this.behaviour.Add(obj);
    }

    /// <summary>
    /// Get an object of type T at a memory address.
    /// </summary>
    public T Get<T>(int pointer)
        => (T)GetObject(pointer);
    
    public int Find(object obj)
    {
        if (this.behaviour is null)
            throw new MemoryBehaviourNotDefined();
        
        return this.behaviour.Find(obj);
    }

    /// <summary>
    /// Get an object at a memory address.
    /// </summary>
    public object GetObject(int pointer)
    {
        if (this.behaviour is null)
            throw new MemoryBehaviourNotDefined();

        return this.behaviour.Get(pointer);
    }

    /// <summary>
    /// Set an object at a memory address with type T.
    /// </summary>
    public void Set<T>(int pointer, T value)
    {
        if (this.behaviour is null)
            throw new MemoryBehaviourNotDefined();
        
        this.behaviour.Set(pointer, value);
        callEvents(pointer);
    }

    void callEvents(int pointer)
    {
        if (!eventDict.ContainsKey(pointer))
            return;
        
        var list = eventDict[pointer];
        foreach (var item in list)
            item?.EventObject?.Awake();
    }

    /// <summary>
    /// Reload all nodes using dependency injection system.
    /// </summary>
    public void Reload()
    {
        this.behaviour.Reload(obj =>
        {
            if (obj is null)
                return null;
            var type = obj.GetType();

            if (!type.Implements("INode"))
                return obj;
            
            return DependencySystem.Current
                .GetConcrete(type);
        });
    }
}