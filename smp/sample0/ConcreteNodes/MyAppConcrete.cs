using System.Collections.Generic;

using Blindness;

[Concrete]
public class MyAppConcrete : MyApp
{
    protected override MyComponentA compA
    {
        get => base.compA;
        set => base.compA = value;
    }
    
    protected override MyComponentB compB
    {
        get => base.compB;
        set => base.compB = value;
    }
}