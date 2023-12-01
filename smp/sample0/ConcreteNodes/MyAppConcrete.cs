using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using Blindness;

[Concrete]
public class MyAppConcrete : Node, MyApp
{
    public MyAppConcrete()
    {
        this.Bind = new MyAppConcreteBinding(this);
    }

    public TableComponent table
    {
        get => BindingSystem.Current.Get<TableComponent>(indexMap[0]);
        set => BindingSystem.Current.Set(indexMap[0], value);
    }

    public InputComponent input
    {
        get => BindingSystem.Current.Get<InputComponent>(indexMap[1]);
        set => BindingSystem.Current.Set(indexMap[1], value);
    }
    public Binding Bind { get; set; }

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

    int[] indexMap = new int[2];
    protected void setBind(int index, int code)
        => indexMap[index] = code;
    protected int getBind(int index)
        => indexMap[index];
    protected int getBindIndex(string field)
        => field switch
        {
            "table" => 0,
            "input" => 1,
            _ => -1
        };
}

public class MyAppConcreteBinding : Binding
{
    public MyAppConcreteBinding(INode node) : base(node) { }

    public TableComponent table { get; set; }

    public InputComponent input { get; set; }
}