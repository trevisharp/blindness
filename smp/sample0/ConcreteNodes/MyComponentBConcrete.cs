using System.Collections.Generic;

using Blindness;

[Concrete]
public class MyComponentBConcrete : MyComponentB
{
    protected override List<string> list
    {
        get
        {
            foreach (var item in indexMap)
                System.Console.WriteLine(item);
            return BindingSystem.Current.Get<List<string>>(indexMap[0]);
        } 
        set => BindingSystem.Current.Set<List<string>>(indexMap[0], value);
    }

    protected override int n
    {
        get => BindingSystem.Current.Get<int>(indexMap[1]);
        set => BindingSystem.Current.Set<int>(indexMap[1], value);
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