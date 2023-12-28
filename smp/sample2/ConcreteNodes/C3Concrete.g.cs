using System.Reflection;

using Blindness;
using Blindness.States;

[Concrete]
public class C3Concrete : Node, C3
{
    public C3Concrete() =>
        this.Bind = new Binding(
            this, 0, typeof(C3),
            s => s switch
            {
        		_ => -1
            }
        );

    

    
}