using System;
using System.Collections.Generic;

namespace Blindness;

public class Memory
{
    private Memory() { }
    private static Memory crr = new();
    public static Memory Current => crr;

    public static void Reset()
        => crr = new();
    
    List<object> data = new List<object>();

    public int Add(object obj)
    {
        data.Add(obj);
        var newIndex = data.Count - 1;

        if (obj is Node node)
            node.MemoryLocation = newIndex;

        print();
        return newIndex;
    }

    public T Get<T>(int index)
    {
        var obj = data[index];
        return (T)obj;
    }

    public void Set<T>(int index, T value)
    {
        data[index] = value;
        print();
    }

    private void print()
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