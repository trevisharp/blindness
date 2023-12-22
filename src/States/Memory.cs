namespace Blindness.States;

using Exceptions;
using Concurrency.Elements;
using System.Collections.Generic;

public class Memory
{
    private IMemoryBehaviour behaviour;
    private Memory(IMemoryBehaviour behaviour)
        => this.behaviour = behaviour;
    private static Memory crr = null;
    public static Memory Current => crr;
    
    Dictionary<int, List<EventElement>> eventDict = new();

    public void AddPointerListner(int pointer, EventElement element)
    {
        if (!eventDict.ContainsKey(pointer))
            eventDict.Add(pointer, new());
        var events = eventDict[pointer];

        events.Add(element);
    }

    public void RemovePointerListner(int pointer, EventElement element)
    {
        if (!eventDict.ContainsKey(pointer))
            return;
        var events = eventDict[pointer];

        events.Remove(element);
        if (events.Count == 0)
            eventDict.Remove(pointer);
    }

    public static void Reset(IMemoryBehaviour behaviour)
        => crr = new(behaviour);

    public int Add(object obj)
    {
        if (this.behaviour is null)
            throw new MemoryBehaviourNotDefined();

        return this.behaviour.Add(obj);
    }

    public T Get<T>(int pointer)
    {
        if (this.behaviour is null)
            throw new MemoryBehaviourNotDefined();

        return this.behaviour.Get<T>(pointer);
    }

    public void Set<T>(int pointer, T value)
    {
        if (this.behaviour is null)
            throw new MemoryBehaviourNotDefined();
        
        this.behaviour.Set<T>(pointer, value);
        callEvents(pointer);
    }

    void callEvents(int pointer)
    {
        if (!eventDict.ContainsKey(pointer))
            return;
        
        var list = eventDict[pointer];
        foreach (var item in list)
            item.Awake();
    }
}