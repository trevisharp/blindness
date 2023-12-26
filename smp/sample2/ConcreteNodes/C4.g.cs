using Blindness;
using Blindness.States;

[Concrete]
public class C4Concrete : Node
{
    public C4Concrete() =>
        this.Bind = new Binding(
            this, 0, typeof(C4),
            s => s switch
            {
                _ => -1
            }
        );
}