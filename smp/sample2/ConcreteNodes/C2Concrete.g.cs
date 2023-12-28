using System.Reflection;

using Blindness;
using Blindness.States;

[Concrete]
public class C2Concrete : Node, C2
{
    public C2Concrete() =>
        this.Bind = new Binding(
            this, 0, typeof(C2),
            s => s switch
            {
        		_ => -1
            }
        );

    

    
}