namespace Blindness.States;

using Elements;

public interface IMemoryBehaviour
{
    int Add(object obj);
    
    T Get<T>(int index);
    void Set<T>(int index, T value);

    void AddPointerListner(int pointer, EventElement element);
    void RemovePointerListner(int pointer, EventElement element);
}