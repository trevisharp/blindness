using System;
using System.Reflection;
using System.Collections.Generic;
using Blindness;
using Blindness.States;

[Concrete]
public class TextBoxConcrete : Node, TextBox
{
	public TextBoxConcrete()
		=> this.Bind = new Binding(
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
	protected override void OnRun()
	    => ((TextBox)this).OnRun();
	public String Title
	{
		get => Bind.Get<String>(0);
		set => Bind.Set(0, value);
	}
	public String Text
	{
		get => Bind.Get<String>(1);
		set => Bind.Set(1, value);
	}
	public Int32 Size
	{
		get => Bind.Get<Int32>(2);
		set => Bind.Set(2, value);
	}
	public Boolean Selected
	{
		get => Bind.Get<Boolean>(3);
		set => Bind.Set(3, value);
	}

}