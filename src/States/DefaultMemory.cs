using System;
using System.Collections.Generic;

namespace Blindness.States;

public class DefaultMemory : IMemoryBehaviour
{ 
    List<object> data = new List<object>();

    public int Add(object obj)
    {
        data.Add(obj);
        var newIndex = data.Count - 1;

        if (obj is Node node)
            node.MemoryLocation = newIndex;
        
        return newIndex;
    }

    public T Get<T>(int index)
    {
        var obj = data[index];
        return (T)obj;
    }

    public void Set<T>(int index, T value)
    {
        lock (data[index])
        {
            data[index] = value;
        }
    }
    
    public void Reload(Func<object, object> func)
    {
        lock (data)
        {
            for (int i = 0; i < data.Count; i++)
                data[i] = func(data[i]);
        }
    }
}