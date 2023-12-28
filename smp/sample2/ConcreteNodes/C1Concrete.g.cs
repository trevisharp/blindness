using System.Reflection;

using Blindness;
using Blindness.States;

[Concrete]
public class C1Concrete : Node, C1
{
    public C1Concrete() =>
        this.Bind = new Binding(
            this, 1, typeof(C1),
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
    var hotReloaded = Blindness.Abstracts.HotReload.Use(this,
        "SSkjykJ5FOuOVR5SpLI4hJRr5yZZFgt96XnIwCrY5i08eM4A3iZuJuY43B9KEn4UGP+5oOlRGYH3aqao8p1tlQ==",
        MethodBase.GetCurrentMethod() as MethodInfo
    );
    if (hotReloaded)
        return;
    
    ((C1)this).OnLoad();
}
protected override void OnRun()
{
    var hotReloaded = Blindness.Abstracts.HotReload.Use(this,
        "SSkjykJ5FOuOVR5SpLI4hJRr5yZZFgt96XnIwCrY5i08eM4A3iZuJuY43B9KEn4UGP+5oOlRGYH3aqao8p1tlQ==",
        MethodBase.GetCurrentMethod() as MethodInfo
    );
    if (hotReloaded)
        return;
    
    ((C1)this).OnRun();
}

}