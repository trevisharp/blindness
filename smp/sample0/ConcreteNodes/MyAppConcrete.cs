using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using Blindness;
using Blindness.States;

[Concrete]
public class MyAppConcrete : Node, MyApp
{
    public MyAppConcrete() =>
        this.Bind = new Binding(
            this, 2, typeof(MyApp), 
            s => s switch
            {
                "table" => 0,
                "input" => 1,
                _ => -1
            }
        );

    public TableComponent table
    {
        get => Bind.Get<TableComponent>(0);
        set => Bind.Set(0, value);
    }

    public InputComponent input
    {
        get => Bind.Get<InputComponent>(1);
        set => Bind.Set(1, value);
    }

    public new void Process()
        => base.Process();

    public void Deps(TableComponent table)
    {
        this.table = table;
    }

    protected override void OnProcess()
        => ((MyApp)this).OnProcess();

    protected override void OnLoad()
        => ((MyApp)this).OnLoad();
}