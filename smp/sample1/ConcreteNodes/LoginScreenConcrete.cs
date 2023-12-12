using Blindness;

[Concrete]
public class LoginScreenConcrete : Node, LoginScreen
{
    public Binding Bind { get; set; }
    public LoginScreenConcrete() =>
        this.Bind = new Binding(
            this, 9, typeof(LoginScreen),
            s => s switch
            {
                "Panel" => 0,
                "Login" => 1,
                "Password" => 2,
                "login" => 3,
                "password" => 4,
                "inLogin" => 5,
                "Repeat" => 6,
                "repeat" => 7,
                _ => -1
            }
        );

    public void Deps(
        Panel Panel, TextBox Login,
        TextBox Password, TextBox Repeat)
    {
        this.Panel = Panel;
        this.Login = Login;
        this.Password = Password;
        this.Repeat = Repeat;
    }

    public Panel Panel
    {
        get => Bind.Get<Panel>(0);
        set => Bind.Set(0, value);
    }
    public TextBox Login
    {
        get => Bind.Get<TextBox>(1);
        set => Bind.Set(1, value);
    }
    public TextBox Password
    {
        get => Bind.Get<TextBox>(2);
        set => Bind.Set(2, value);
    }
    public string login
    {
        get => Bind.Get<string>(3);
        set => Bind.Set(3, value);
    }
    public string password
    {
        get => Bind.Get<string>(4);
        set => Bind.Set(4, value);
    }
    public int selectedField
    {
        get => Bind.Get<int>(5);
        set => Bind.Set(5, value);
    }
    public TextBox Repeat
    {
        get => Bind.Get<TextBox>(6);
        set => Bind.Set(6, value);
    }
    public string repeat
    {
        get => Bind.Get<string>(7);
        set => Bind.Set(7, value);
    }

    protected override void OnProcess()
        => ((LoginScreen)this).OnProcess();

    protected override void OnLoad()
        => ((LoginScreen)this).OnLoad();

}