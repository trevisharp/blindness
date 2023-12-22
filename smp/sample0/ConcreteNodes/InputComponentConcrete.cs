using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

using Blindness;
using Blindness.States;

[Concrete]
public class InputComponentConcrete : Node, InputComponent
{
    public InputComponentConcrete() =>
        this.Bind = new Binding(
            this, 2, typeof(InputComponent),
            s => s switch
            {
                "list" => 0,
                "n" => 1,
                _ => -1
            }
        );
    
    public List<string> list
    {
        get => Bind.Get<List<string>>(0);
        set => Bind.Set(0, value);
    }

    public int n
    {
        get => Bind.Get<int>(1);
        set => Bind.Set(1, value);
    }

    public new void Process()
        => base.Process();
}