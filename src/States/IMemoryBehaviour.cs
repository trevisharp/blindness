using System;

namespace Blindness.States;

public interface IMemoryBehaviour
{
    int Add(object obj);
    object Get(int index);
    void Set(int index, object value);
    void Reload(Func<object, object> func);
}