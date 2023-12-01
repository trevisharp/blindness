using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using Blindness;

[Concrete]
public class TableComponentConcrete : Node, TableComponent
{
    public Binding Bind { get; set; }
    public TableComponentConcrete()
    {
        this.Bind = new TableComponentConcreteBinding(this);
    }
    public int size
    {
        get => BindingSystem.Current.Get<int>(indexMap[0]);
        set => BindingSystem.Current.Set(indexMap[0], value);
    }

    public List<string> texts
    {
        get => BindingSystem.Current.Get<List<string>>(indexMap[1]);
        set => BindingSystem.Current.Set(indexMap[1], value);
    }

    public InputComponent input
    {
        get => BindingSystem.Current.Get<InputComponent>(indexMap[2]);
        set => BindingSystem.Current.Set(indexMap[2], value);
    }
    
    public InputItemComponent itemInput
    {
        get => BindingSystem.Current.Get<InputItemComponent>(indexMap[3]);
        set => BindingSystem.Current.Set(indexMap[3], value);
    }

    public InputCommandComponent commandInput
    {
        get => BindingSystem.Current.Get<InputCommandComponent>(indexMap[4]);
        set => BindingSystem.Current.Set(indexMap[4], value);
    }

    public new void Process()
        => base.Process();

    protected override void OnProcess()
        => ((TableComponent)this).OnProcess();

    protected override void OnLoad()
        => ((TableComponent)this).OnLoad();

    int[] indexMap = new int[5];
    protected void setBind(int index, int code)
        => indexMap[index] = code;
    protected int getBind(int index)
        => indexMap[index];
    protected int getBindIndex(string field)
        => field switch
        {
            "size" => 0,
            "texts" => 1,
            "input" => 2,
            "itemInput" => 3,
            "commandInput" => 4,
            _ => -1
        };

    public void Deps(InputItemComponent itemInput, InputCommandComponent commandInput)
    {
        this.itemInput = itemInput;
        this.commandInput = commandInput;
    }
}

public class TableComponentConcreteBinding : Binding
{
    public TableComponentConcreteBinding(INode node) : base(node) { }
    public int size { get; set; }

    public List<string> texts { get; set; }

    public InputComponent input { get; set; }
    
    public InputItemComponent itemInput { get; set; }

    public InputCommandComponent commandInput { get; set; }
}