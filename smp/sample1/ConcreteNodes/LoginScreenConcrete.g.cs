using System.Reflection;

using Blindness;
using Blindness.States;

[Concrete]
public class LoginScreenConcrete : Node, LoginScreen
{
    public LoginScreenConcrete() =>
        this.Bind = new Binding(
            this, 8, typeof(LoginScreen),
            s => s switch
            {
        		"Panel" => 0,
		"Password" => 2,
		"login" => 4,
		"repeat" => 6,
		_ => -1
            }
        );

    
    public Panel Panel
    {
        get => Bind.Get<Panel>(0);
        set => Bind.Set(0, value);
    }

    public TextBox Password
    {
        get => Bind.Get<TextBox>(2);
        set => Bind.Set(2, value);
    }

    public String login
    {
        get => Bind.Get<String>(4);
        set => Bind.Set(4, value);
    }

    public String repeat
    {
        get => Bind.Get<String>(6);
        set => Bind.Set(6, value);
    }


    public void Deps(
	Panel Panel,
	TextBox Login,
	TextBox Password,
	TextBox Repeat
)
{
this.Panel = Panel;
this.Login = Login;
this.Password = Password;
this.Repeat = Repeat;
}
protected override void OnLoad()
{
    ((LoginScreen)this).OnLoad();
}

}