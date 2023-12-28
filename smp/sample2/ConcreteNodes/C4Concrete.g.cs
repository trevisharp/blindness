using System.Reflection;

using Blindness;
using Blindness.States;

[Concrete]
public class C4Concrete : Node, C4
{
    public C4Concrete() =>
        this.Bind = new Binding(
            this, 1, typeof(C4),
            s => s switch
            {
        		"MyField" => 0,
		_ => -1
            }
        );

    
    public C2 MyField
    {
        get => Bind.Get<C2>(0);
        set => Bind.Set(0, value);
    }


    public void Deps(
	C2 MyField
)
{
this.MyField = MyField;
}
protected override void OnLoad()
{
    ((C4)this).OnLoad();
}
protected override void OnRun()
{
    ((C4)this).OnRun();
}

}