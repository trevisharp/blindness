using System;
using System.Collections.Generic;

namespace Blindness.States;

using Elements;

public class Memory
{
    private Memory() { }
    private static Memory crr = new();
    public static Memory Current => crr;

    public static void Reset()
        => crr = new();
    
    List<object> data = new List<object>();
    Dictionary<int, List<EventElement>> eventDict = new();

    public void AddPointerListner(int pointer, EventElement element)
    {
        if (!eventDict.ContainsKey(pointer))
            eventDict.Add(pointer, new());
        var events = eventDict[pointer];

        events.Add(element);
    }

    public void RemovePointerListner(int pointer, EventElement element)
    {
        if (!eventDict.ContainsKey(pointer))
            return;
        var events = eventDict[pointer];

        events.Remove(element);
        if (events.Count == 0)
            eventDict.Remove(pointer);
    }

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
        
        if (!eventDict.ContainsKey(index))
            return;
        
        var list = eventDict[index];
        foreach (var item in list)
            item.Awake();
    }

    internal void print()
    {
        Console.ForegroundColor = ConsoleColor.Green;
        int i = 0;
        Console.WriteLine("Memory State");
        foreach (var item in data)
        {
            Console.WriteLine(
                $"data[{i++}] = {item}"
            );
        }
        System.Console.WriteLine();
        Console.ResetColor();
    }
}