using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using Blindness;

[Concrete]
public class TableComponentConcrete : Node, TableComponent
{
    public Binding Bind { get; set; }
    public TableComponentConcrete() =>
        this.Bind = new Binding(
            this, 5, typeof(TableComponent),
            s => s switch
            {
                "list" => 0,
                "n" => 1,
                _ => -1
            }
        );
    public int size
    {
        get => Bind.Get<int>(0);
        set => Bind.Set(0, value);
    }

    public List<string> texts
    {
        get => Bind.Get<List<string>>(1);
        set => Bind.Set(1, value);
    }

    public InputComponent input
    {
        get => Bind.Get<InputComponent>(2);
        set => Bind.Set(2, value);
    }
    
    public InputItemComponent itemInput
    {
        get => Bind.Get<InputItemComponent>(3);
        set => Bind.Set(3, value);
    }

    public InputCommandComponent commandInput
    {
        get => Bind.Get<InputCommandComponent>(4);
        set => Bind.Set(4, value);
    }

    public new void Process()
        => base.Process();

    protected override void OnProcess()
        => ((TableComponent)this).OnProcess();

    protected override void OnLoad()
        => ((TableComponent)this).OnLoad();

    public void Deps(
        InputItemComponent itemInput,
        InputCommandComponent commandInput
    )
    {
        this.itemInput = itemInput;
        this.commandInput = commandInput;
    }
}