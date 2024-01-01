using System;
using System.Reflection;
using System.Collections.Generic;
using Blindness;
using Blindness.States;

[Concrete]
public class LoginScreenConcrete : Node, LoginScreen
{
	public LoginScreenConcrete()
		=> this.Bind = new Binding(
			this, 8, typeof(LoginScreen),
			s => s switch
			{
				"Panel" => 0,
				"Login" => 1,
				"Password" => 2,
				"Repeat" => 3,
				"login" => 4,
				"password" => 5,
				"repeat" => 6,
				"selectedField" => 7,
				_ => -1
			}
		);
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
	    => ((LoginScreen)this).OnLoad();
	protected override void OnRun()
	    => ((LoginScreen)this).OnRun();
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
	public TextBox Repeat
	{
		get => Bind.Get<TextBox>(3);
		set => Bind.Set(3, value);
	}
	public String login
	{
		get => Bind.Get<String>(4);
		set => Bind.Set(4, value);
	}
	public String password
	{
		get => Bind.Get<String>(5);
		set => Bind.Set(5, value);
	}
	public String repeat
	{
		get => Bind.Get<String>(6);
		set => Bind.Set(6, value);
	}
	public Int32 selectedField
	{
		get => Bind.Get<Int32>(7);
		set => Bind.Set(7, value);
	}

}