namespace Blindness.States;

using Exceptions;
using Concurrency.Elements;

public class Memory
{
    private IMemoryBehaviour behaviour;
    private Memory(IMemoryBehaviour behaviour)
        => this.behaviour = behaviour;
    private static Memory crr = null;
    public static Memory Current => crr;

    public static void Reset(IMemoryBehaviour behaviour)
        => crr = new(behaviour);
    
    public void AddPointerListner(int pointer, EventElement element)
    {
        if (this.behaviour is null)
            throw new MemoryBehaviourNotDefined();

        this.behaviour.AddPointerListner(pointer, element);
    }

    public void RemovePointerListner(int pointer, EventElement element)
    {
        if (this.behaviour is null)
            throw new MemoryBehaviourNotDefined();

        this.behaviour.RemovePointerListner(pointer, element);
    }

    public int Add(object obj)
    {
        if (this.behaviour is null)
            throw new MemoryBehaviourNotDefined();

        return this.behaviour.Add(obj);
    }

    public T Get<T>(int index)
    {
        if (this.behaviour is null)
            throw new MemoryBehaviourNotDefined();

        return this.behaviour.Get<T>(index);
    }

    public void Set<T>(int index, T value)
    {
        if (this.behaviour is null)
            throw new MemoryBehaviourNotDefined();
        
        this.behaviour.Set<T>(index, value);
    }
}