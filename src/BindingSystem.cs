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
        // System.Console.WriteLine($"Data Add: {obj} at {data.Count}");
        data.Add(obj);
        return data.Count - 1;
    }

    public T Get<T>(int index)
    {
        var obj = data[index];
        if (obj is PreInitNode init)
        {
            obj = data[index] = DependencySystem.Current
                .GetConcrete(init.RealType);
            // System.Console.WriteLine($"Data Auto Replace: {obj} at {index}");
        }

        // System.Console.WriteLine($"Data Get: {obj} at {index}");
        return (T)obj;
    }

    public void Set<T>(int index, T value)
    {
        // System.Console.WriteLine($"Data Set: {value} at {index}");
        data[index] = value;
    }
}