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

    public object Get(int index)
    {
        return data[index];
    }

    public void Set(int index, object value)
    {
        data[index] = value;
    }
}