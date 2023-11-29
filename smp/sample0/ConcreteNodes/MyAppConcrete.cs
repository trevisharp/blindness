using System.Collections.Generic;

using Blindness;

[Concrete]
public class MyAppConcrete : MyApp, IConcrete
{
    int[] indexMap = new int[2];

    protected override MyComponentA compA
    {
        get => (MyComponentA)BindingSystem.Current.Get(indexMap[0]);
        set => BindingSystem.Current.Set(indexMap[0], value);
    }
    
    protected override MyComponentB compB
    {
        get => (MyComponentB)BindingSystem.Current.Get(indexMap[1]);
        set => BindingSystem.Current.Set(indexMap[1], value);
    }
}