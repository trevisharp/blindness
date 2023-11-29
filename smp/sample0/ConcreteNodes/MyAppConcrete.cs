using System.Collections.Generic;

using Blindness;

[Concrete]
public class MyAppConcrete : MyApp
{
    protected override MyComponentA compA
    {
        get => BindingSystem.Current.Get<MyComponentA>(indexMap[0]);
        set => BindingSystem.Current.Set<MyComponentA>(indexMap[0], value);
    }
    
    protected override MyComponentB compB
    {
        get => BindingSystem.Current.Get<MyComponentB>(indexMap[1]);
        set => BindingSystem.Current.Set<MyComponentB>(indexMap[1], value);
    }
    
    int[] indexMap = new int[2];
    void setBind(int index, int code)
        => indexMap[index] = code;
    int getBind(int index)
        => indexMap[index];
    int getBindIndex(string field)
        => field switch
        {
            "compA" => 0,
            "compB" => 1,
            _ => -1
        };
}