using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace Blindness.States;

using Internal;
using Exceptions;
using Concurrency.Elements;
using System.Reflection;

public class Memory
{
    private IMemoryBehaviour behaviour;
    private Memory(IMemoryBehaviour behaviour)
        => this.behaviour = behaviour;
    private static Memory crr = null;
    public static Memory Current => crr;
    
    Dictionary<int, List<PointerListner>> eventDict = new();

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
            item?.EventObject?.Awake();
    }

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