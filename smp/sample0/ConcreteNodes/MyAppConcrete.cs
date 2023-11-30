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
        get => BindingSystem.Current.Get<InputComponent>(indexMap[1]);
        set => BindingSystem.Current.Set(indexMap[1], value);
    }

    int[] indexMap = new int[2];
    protected void setBind(int index, int code)
        => indexMap[index] = code;
    protected int getBind(int index)
        => indexMap[index];
    protected int getBindIndex(string field)
        => field switch
        {
            "table" => 0,
            "input" => 1,
            _ => -1
        };
}