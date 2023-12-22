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
                "Text" => 1,
                "Size" => 2,
                "Selected" => 3,
                _ => -1
            }
        );
    public string Title
    {
        get => Bind.Get<string>(0);
        set => Bind.Set(0, value);
    }
    public string Text
    {
        get => Bind.Get<string>(1);
        set => Bind.Set(1, value);
    }
    public int Size
    {
        get => Bind.Get<int>(2);
        set => Bind.Set(2, value);
    }
    public bool Selected
    {
        get => Bind.Get<bool>(3);
        set => Bind.Set(3, value);
    }

    protected override void OnRun()
        => ((TextBox)this).OnProcess();
}