using System.Reflection;

using Blindness;
using Blindness.States;

[Concrete]
public class PanelConcrete : Node, Panel
{
    public PanelConcrete() =>
        this.Bind = new Binding(
            this, 3, typeof(Panel),
            s => s switch
            {
        		"Title" => 0,
		"Children" => 2,
		_ => -1
            }
        );

    
    public String Title
    {
        get => Bind.Get<String>(0);
        set => Bind.Set(0, value);
    }

    public List`1 Children
    {
        get => Bind.Get<List`1>(2);
        set => Bind.Set(2, value);
    }


    
}