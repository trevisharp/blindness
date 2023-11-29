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
        return data.Count - 1;
    }

    public T Get<T>(int index)
    {
        return (T)data[index];
    }

    public void Set<T>(int index, T value)
    {
        data[index] = value;
    }
}