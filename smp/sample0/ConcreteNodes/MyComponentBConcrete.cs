using System.Collections.Generic;

using Blindness;

[Concrete]
public class MyComponentBConcrete : MyComponentB
{
    protected override List<string> list
    {
        get => base.list;
        set => base.list = value;
    }

    protected override int n
    {
        get => base.n;
        set => base.n = value;
    }
}