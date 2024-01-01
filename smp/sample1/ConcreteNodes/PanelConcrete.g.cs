using System;
using System.Reflection;
using System.Collections.Generic;
using Blindness;
using Blindness.States;

[Concrete]
public class PanelConcrete : Node, Panel
{
	public PanelConcrete()
		=> this.Bind = new Binding(
			this, 3, typeof(Panel),
			s => s switch
			{
				"Title" => 0,
				"Width" => 1,
				"Children" => 2,
				_ => -1
			}
		);
	protected override void OnRun()
	    => ((Panel)this).OnRun();
	public String Title
	{
		get => Bind.Get<String>(0);
		set => Bind.Set(0, value);
	}
	public Int32 Width
	{
		get => Bind.Get<Int32>(1);
		set => Bind.Set(1, value);
	}
	public List<INode> Children
	{
		get => Bind.Get<List<INode>>(2);
		set => Bind.Set(2, value);
	}

}