using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using Blindness;

[Concrete]
public class InputCommandComponentConcrete : Node, InputCommandComponent
{
    public List<string> list
    {
        get => BindingSystem.Current.Get<List<string>>(indexMap[0]);
        set => BindingSystem.Current.Set(indexMap[0], value);
    }

    public int n
    {
        get => BindingSystem.Current.Get<int>(indexMap[1]);
        set => BindingSystem.Current.Set(indexMap[1], value);
    }

    public new void Process()
        => base.Process();

    public dynamic Bind(Expression<Func<object, object>> binding)
    {
        base.Bind(binding);
        return this;
    }

    protected override void OnProcess()
        => ((InputCommandComponent)this).OnProcess();

    int[] indexMap = new int[2];
    public void setBind(int index, int code)
        => indexMap[index] = code;
    public int getBind(int index)
        => indexMap[index];
    public int getBindIndex(string field)
        => field switch
        {
            "list" => 0,
            "n" => 1,
            _ => -1
        };
}