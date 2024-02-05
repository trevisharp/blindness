/* Author:  Leonardo Trevisan Silio
 * Date:    05/02/2024
 */
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace Blindness.States;

/// <summary>
/// Default implementation of memory.
/// </summary>
public class DefaultMemory : IMemoryBehaviour
{
    int nextIndex = 0;
    int nextVectorIndex = vectorLen;
    const int vectorLen = 64;
    LinkedList<object[]> data = new();

    public int Add(object obj)
    {
        add(obj);
        var newIndex = nextIndex;
        nextIndex++;

        if (obj is Node node)
            node.MemoryLocation = newIndex;
        
        return newIndex;
    }

    public object Get(int pointer)
    {
        var bucket = getBucket(ref pointer);
        return bucket[pointer];
    }

    public void Set(int pointer, object value)
    {
        var bucket = getBucket(ref pointer);
        lock (bucket)
        {
            bucket[pointer] = value;
        }
    }

    public int Find(object value)
    {
        var it = data.First;
        int pointer = 0;
        while (it is not null)
        {
            var bucket = it.Value;
            for (int i = 0; i < vectorLen; i++, pointer++)
                if (bucket[i] == value)
                    return pointer;
        }

        return -1;
    }
    
    public void Reload(Func<object, object> func)
    {
        lock (data)
        {
            var it = data.First;
            var bucket = it.Value;
            for (int i = 0; i < nextIndex; i++)
            {
                int index = i % vectorLen;
                bucket[index] = func(bucket[index]);

                if (i % vectorLen < vectorLen - 1)
                    continue;
                
                it = it.Next;
                bucket = it.Value;
            }
        }
    }

    void add(object obj)
    {
        var bucket = getCurrentBucket();
        bucket[nextVectorIndex] = obj;
        nextVectorIndex++;
    }

    object[] getCurrentBucket()
    {
        if (nextVectorIndex < vectorLen)
            return data.Last.Value;
        
        nextVectorIndex = 0;
        var newArray = new object[vectorLen];
        data.AddLast(newArray);
        return newArray;
    }

    object[] getBucket(ref int pointer)
    {
        var node = data.First;
        while (pointer > vectorLen)
        {
            node = node.Next;
            pointer -= vectorLen;
        }
        
        return node.Value;
    }
}