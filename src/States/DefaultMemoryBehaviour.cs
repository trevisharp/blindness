/* Author:  Leonardo Trevisan Silio
 * Date:    16/07/2024
 */
using System;
using System.Collections.Generic;

namespace Blindness.States;

/// <summary>
/// Default implementation of memory.
/// </summary>
public class DefaultMemoryBehaviour : IMemoryBehaviour
{
    int nextIndex = 0;
    int currentBucketIndex = vectorLen;
    const int vectorLen = 64;
    readonly LinkedList<object[]> data = [];

    public int Add(object obj)
    {
        AddToBucket(obj);
        var newIndex = nextIndex;
        nextIndex++;

        if (obj is Node node)
            node.MemoryLocation = newIndex;
        
        return newIndex;
    }

    public object Get(int pointer)
    {
        var bucket = GetBucket(ref pointer);
        return bucket[pointer];
    }

    public void Set(int pointer, object value)
    {
        var bucket = GetBucket(ref pointer);
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

                if (index < vectorLen - 1)
                    continue;
                
                it = it.Next;
                bucket = it.Value;
            }
        }
    }

    /// <summary>
    /// Add element to last position on the last bucket.
    /// Create bucket if list is full.
    /// </summary>
    void AddToBucket(object obj)
    {
        var bucket = GetLastBucket();
        bucket[currentBucketIndex] = obj;
        currentBucketIndex++;
    }

    /// <summary>
    /// Get Last Bucket. If the last bucket is full, create a
    /// new bucket and return it.
    /// </summary>
    /// <returns></returns>
    object[] GetLastBucket()
    {
        if (currentBucketIndex < vectorLen)
            return data.Last.Value;
        
        currentBucketIndex = 0;
        var newBucket = new object[vectorLen];
        data.AddLast(newBucket);
        return newBucket;
    }

    /// <summary>
    /// Get bucket that contains a pointer and set the
    /// pointer to a index in bucket referential.
    /// </summary>
    object[] GetBucket(ref int pointer)
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