using System.Collections.Generic;

using Blindness;

[Concrete]
public class MyComponentAConcrete : MyComponentA
{   
    protected override int size
    {
        get => BindingSystem.Current.Get<int>(indexMap[0]);
        set => BindingSystem.Current.Set<int>(indexMap[0], value);
    }

    protected override List<string> texts
    {
        get => BindingSystem.Current.Get<List<string>>(indexMap[1]);
        set => BindingSystem.Current.Set<List<string>>(indexMap[1], value);
    }

    protected override MyComponentB compB
    {
        get => BindingSystem.Current.Get<MyComponentB>(indexMap[2]);
        set => BindingSystem.Current.Set<MyComponentB>(indexMap[2], value);
    }
    int[] indexMap = new int[3];
    void setBind(int index, int code)
        => indexMap[index] = code;
    int getBind(int index)
        => indexMap[index];
    int getBindIndex(string field)
        => field switch
        {
            "size" => 0,
            "texts" => 1,
            "compB" => 2,
            _ => -1
        };
}