/* Author:  Leonardo Trevisan Silio
 * Date:    07/08/2024
 */
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Blindness.Bind;

/// <summary>
/// A Cache to virtual associate objects to theiers bind without
/// affect the garbarage collector.
/// </summary>
public class BindingCache
{
    class Node
    {
        public KeyValuePair<WeakReference, Binding> Value { get; set; }
        public Node Next { get; set; }
        public Node Previous { get; set; }
    } 

    class Bucket
    {
        public Node First { get; set; }
        public Node Last { get; set; }
    }

    int count = 0;
    int bucketCount = 4;
    const int limitToRebuild = 8;

    Bucket[] buckets = [ new(), new(), new(), new() ];
    
    public Binding this[object obj]
    {
        get => GetOrAdd(obj);
        set => SetOrAdd(obj, value);
    }

    void RebuildIfAboveLimit()
    {
        if (count > limitToRebuild * bucketCount)
            Rebuild();
    }

    void Rebuild()
    {
        var oldBuckets = buckets;

        bucketCount *= 2;
        buckets = new Bucket[bucketCount];
        for (int i = 0; i < bucketCount; i++)
            buckets[i] = new();
        
        foreach (var bucket in oldBuckets)
        {
            var node = bucket.First;
            while (node is not null)
            {
                this[node.Value.Key] = node.Value.Value;
                node = node.Next;
            }
        }
    }

    Bucket GetBucket(object obj)
    {
        var hash = RuntimeHelpers.GetHashCode(obj);
        var bucketId = hash % bucketCount;
        var bucket = buckets[bucketId];
        return bucket;
    }

    void ClearDeadNodes(Bucket bucket)
    {
        var node = bucket.First;
        while (node is not null)
        {
            var pair = node.Value;
            var weakref = pair.Key;
            var prev = node.Previous;
            var next = node.Next;

            if (weakref.IsAlive)
            {
                node = next;
                continue;
            }
            
            count--;
            
            if (node == bucket.First)
            {
                bucket.First = node = next;
                continue;
            }

            prev.Next = next;
            node = next;
        }
    }

    void AddNode(Bucket bucket, Node newNode)
    {
        count++;

        if (bucket.First is null)
        {
            bucket.First = bucket.Last = newNode;
            return;
        }

        bucket.Last.Next = newNode;
        newNode.Previous = bucket.Last;
        bucket.Last = newNode;
    }

    void SetOrAdd(object obj, Binding bind)
    {
        RebuildIfAboveLimit();
        
        var bucket = GetBucket(obj);
        ClearDeadNodes(bucket);
        
        var node = bucket.First;
        while (node is not null)
        {
            var pair = node.Value;
            var weakref = pair.Key;

            var target = weakref.Target;
            if (ReferenceEquals(target, obj))
                node.Value = new(weakref, bind);

            node = node.Next;
        }

        var newBinding = new Binding();
        AddNode(bucket, new Node {
            Value = new(new WeakReference(obj), newBinding)
        });
    }

    Binding GetOrAdd(object obj)
    {
        RebuildIfAboveLimit();
        
        var bucket = GetBucket(obj);
        ClearDeadNodes(bucket);
        
        var node = bucket.First;
        while (node is not null)
        {
            var pair = node.Value;
            var weakref = pair.Key;

            var target = weakref.Target;
            if (ReferenceEquals(target, obj))
                return pair.Value;

            node = node.Next;
        }

        var newBinding = new Binding();
        AddNode(bucket, new Node {
            Value = new(new WeakReference(obj), newBinding)
        });
        return newBinding;
    }
}