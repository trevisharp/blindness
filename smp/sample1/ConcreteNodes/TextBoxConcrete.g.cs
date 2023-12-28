using System.Reflection;

using Blindness;
using Blindness.States;

[Concrete]
public class TextBoxConcrete : Node, TextBox
{
    public TextBoxConcrete() =>
        this.Bind = new Binding(
            this, 4, typeof(TextBox),
            s => s switch
            {
        		"Title" => 0,
		"Size" => 2,
		_ => -1
            }
        );

    
    public String Title
    {
        get => Bind.Get<String>(0);
        set => Bind.Set(0, value);
    }

    public Int32 Size
    {
        get => Bind.Get<Int32>(2);
        set => Bind.Set(2, value);
    }


    
}