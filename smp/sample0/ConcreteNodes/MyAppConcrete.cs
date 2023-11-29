using System.Collections.Generic;

using Blindness;

[Concrete]
public class MyAppConcrete : MyApp, IConcrete
{
    int[] indexMap = new int[2];
    public void SetBind(int index, int code)
        => indexMap[index] = code;

    protected override MyComponentA compA
    {
        get => BindingSystem.Current.Get<MyComponentA>(indexMap[0]);
        set => BindingSystem.Current.Set<MyComponentA>(indexMap[0], value);
    }
    
    protected override MyComponentB compB
    {
        get => BindingSystem.Current.Get<MyComponentB>(indexMap[1]);
        set => BindingSystem.Current.Set<MyComponentB>(indexMap[1], value);
    }
}