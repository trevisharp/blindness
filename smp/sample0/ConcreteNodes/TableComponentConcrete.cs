using System.Collections.Generic;

using Blindness;

[Concrete]
public class TableComponentConcrete : TableComponent
{   
    protected override int size
    {
        get => BindingSystem.Current.Get<int>(indexMap[0]);
        set => BindingSystem.Current.Set(indexMap[0], value);
    }

    protected override List<string> texts
    {
        get => BindingSystem.Current.Get<List<string>>(indexMap[1]);
        set => BindingSystem.Current.Set(indexMap[1], value);
    }

    protected override InputComponent input
    {
        get => BindingSystem.Current.Get<InputComponent>(indexMap[2]);
        set => BindingSystem.Current.Set(indexMap[2], value);
    }
    
    protected override InputItemComponent itemInput
    {
        get => BindingSystem.Current.Get<InputItemComponent>(indexMap[1]);
        set => BindingSystem.Current.Set(indexMap[3], value);
    }

    protected override InputCommandComponent commandInput
    {
        get => BindingSystem.Current.Get<InputCommandComponent>(indexMap[2]);
        set => BindingSystem.Current.Set(indexMap[4], value);
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
            "input" => 2,
            "commandInput" => 3,
            "itemInput" => 4,
            _ => -1
        };
}