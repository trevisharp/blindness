/* Author:  Leonardo Trevisan Silio
 * Date:    01/01/2024
 */
using System;
using System.Collections.Generic;

namespace Blindness.States;

/// <summary>
/// Default implementation of memory.
/// </summary>
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

    public object Get(int index)
    {
        var obj = data[index];
        return obj;
    }

    public void Set(int index, object value)
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