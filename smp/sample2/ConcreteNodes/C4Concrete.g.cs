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
    var hotReloaded = Blindness.Abstracts.HotReload.Use(this,
        "t+JtvWi6a4ctThn7nPgS4AIPmixgR4lVVXnlz9M+fDv+JL+y9cS4qtSadraK/Cd1yiBRdaIWE6jDxSc0FPKixg==",
        MethodBase.GetCurrentMethod() as MethodInfo
    );
    if (hotReloaded)
        return;
    
    ((C4)this).OnLoad();
}
protected override void OnRun()
{
    var hotReloaded = Blindness.Abstracts.HotReload.Use(this,
        "t+JtvWi6a4ctThn7nPgS4AIPmixgR4lVVXnlz9M+fDv+JL+y9cS4qtSadraK/Cd1yiBRdaIWE6jDxSc0FPKixg==",
        MethodBase.GetCurrentMethod() as MethodInfo
    );
    if (hotReloaded)
        return;
    
    ((C4)this).OnRun();
}

}