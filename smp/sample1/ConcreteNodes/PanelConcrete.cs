using System.Collections.Generic;
using Blindness;

[Concrete]
public class PanelConcrete : Node, Panel
{
    public Binding Bind { get; set; }
    public PanelConcrete() =>
        this.Bind = new Binding(
            this, 3, typeof(Panel),
            s => s switch
            {
                "Title" => 0,
                "Width" => 1,
                "Children" => 2,
                _ => -1
            }
        );
    
    public string Title
    {
        get => Bind.Get<string>(0);
        set => Bind.Set(0, value);
    }
    public int Width
    {
        get => Bind.Get<int>(1);
        set => Bind.Set(1, value);
    }
    public List<INode> Children
    {
        get => Bind.Get<List<INode>>(2);
        set => Bind.Set(2, value);
    }

    public new void Process()
        => base.Process();

    protected override void OnProcess()
        => ((Panel)this).OnProcess();

}