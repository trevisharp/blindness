using System.Collections.Generic;

using Blindness;

[Concrete]
public class MyComponentAConcrete : MyComponentA
{
    protected override int size
    {
        get => base.size;
        set => base.size = value;
    }

    protected override List<string> texts
    {
        get => base.texts;
        set => base.texts = value;
    }

    protected override MyComponentB compB
    { 
        get => base.compB; 
        set => base.compB = value;
    }
}