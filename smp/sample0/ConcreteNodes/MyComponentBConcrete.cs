using System.Collections.Generic;

using Blindness;

[Concrete]
public class MyComponentBConcrete : MyComponentB, IConcrete
{
    int[] indexMap = new int[2];
    
    protected override List<string> list
    {
        get => (List<string>)BindingSystem.Current.Get(indexMap[0]);
        set => BindingSystem.Current.Set(indexMap[0], value);
    }

    protected override int n
    {
        get => (int)BindingSystem.Current.Get(indexMap[1]);
        set => BindingSystem.Current.Set(indexMap[1], value);
    }
}