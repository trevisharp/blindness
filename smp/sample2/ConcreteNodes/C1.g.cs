using Blindness;
using Blindness.States;

[Concrete]
public class C1Concrete : Node
{
    public C1Concrete() =>
        this.Bind = new Binding(
            this, 0, typeof(C1),
            s => s switch
            {
                _ => -1
            }
        );
}