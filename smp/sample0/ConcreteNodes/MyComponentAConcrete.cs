using System.Collections.Generic;

using Blindness;

[Concrete]
public class MyComponentAConcrete : MyComponentA, IConcrete
{
    int[] indexMap = new int[3];
    
    protected override int size
    {
        get => (int)BindingSystem.Current.Get(indexMap[0]);
        set => BindingSystem.Current.Set(indexMap[0], value);
    }

    protected override List<string> texts
    {
        get => (List<string>)BindingSystem.Current.Get(indexMap[1]);
        set => BindingSystem.Current.Set(indexMap[1], value);
    }

    protected override MyComponentB compB
    {
        get => (MyComponentB)BindingSystem.Current.Get(indexMap[2]);
        set => BindingSystem.Current.Set(indexMap[2], value);
    }
}