using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using Blindness;

[Concrete]
public class InputCommandComponentConcrete : Node, InputCommandComponent
{
    public Binding Bind { get; set; }
    public InputCommandComponentConcrete() =>
        this.Bind = new Binding(
            this, 2, s => s switch
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

    protected override void OnProcess()
        => ((InputCommandComponent)this).OnProcess();
}