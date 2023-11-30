using System.Collections.Generic;

using Blindness;

[Concrete]
public class InputComponentConcrete : InputComponent
{
    protected override List<string> list
    {
        get => BindingSystem.Current.Get<List<string>>(indexMap[0]);
        set => BindingSystem.Current.Set(indexMap[0], value);
    }

    protected override int n
    {
        get => BindingSystem.Current.Get<int>(indexMap[1]);
        set => BindingSystem.Current.Set(indexMap[1], value);
    }

    int[] indexMap = new int[2];
    void setBind(int index, int code)
        => indexMap[index] = code;
    int getBind(int index)
        => indexMap[index];
    int getBindIndex(string field)
        => field switch
        {
            "list" => 0,
            "n" => 1,
            _ => -1
        };
}