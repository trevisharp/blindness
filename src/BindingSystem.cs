using System;
using System.Collections.Generic;

namespace Blindness;

public class BindingSystem
{
    private BindingSystem() { }
    private static BindingSystem crr = new();
    public static BindingSystem Current => crr;

    public static void Reset()
        => crr = new();
    
    List<object> data = new List<object>();

    public int Add(object obj)
    {
        data.Add(obj);
        print();
        return data.Count - 1;
    }

    public T Get<T>(int index)
    {
        var obj = data[index];
        if (obj is PreInitNode init)
        {
            obj = data[index] = DependencySystem.Current
                .GetConcrete(init.RealType);
            print();
        }

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