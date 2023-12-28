using System;

namespace Blindness.States;

public interface IMemoryBehaviour
{
    int Add(object obj);
    T Get<T>(int index);
    void Set<T>(int index, T value);
    void Reload(Func<object, object> func);
}