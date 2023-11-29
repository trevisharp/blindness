using System.Collections.Generic;

using Blindness;

[Concrete]
public class MyComponentBConcrete : MyComponentB, IConcrete
{
    int[] indexMap = new int[2];
    public void SetBind(int index, int code)
        => indexMap[index] = code;
    
    protected override List<string> list
    {
        get => BindingSystem.Current.Get<List<string>>(indexMap[0]);
        set => BindingSystem.Current.Set<List<string>>(indexMap[0], value);
    }

    protected override int n
    {
        get => BindingSystem.Current.Get<int>(indexMap[1]);
        set => BindingSystem.Current.Set<int>(indexMap[1], value);
    }
}