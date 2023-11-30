using System.Collections.Generic;

using Blindness;

[Concrete]
public class MyAppConcrete : MyApp
{
    protected override TableComponent table
    {
        get => BindingSystem.Current.Get<TableComponent>(indexMap[0]);
        set => BindingSystem.Current.Set(indexMap[0], value);
    }

    protected override InputComponent input
    {
        get => BindingSystem.Current.Get<InputComponent>(indexMap[3]);
        set => BindingSystem.Current.Set(indexMap[3], value);
    }

    int[] indexMap = new int[4];
    void setBind(int index, int code)
        => indexMap[index] = code;
    int getBind(int index)
        => indexMap[index];
    int getBindIndex(string field)
        => field switch
        {
            "table" => 0,
            "itemInput" => 1,
            _ => -1
        };
}