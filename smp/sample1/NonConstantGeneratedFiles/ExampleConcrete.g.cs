//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Reflection;
using System.Collections.Generic;
using Blindness;
using Blindness.Bind;
using Blindness.Core;

[Concrete]
public partial class ExampleConcrete : Node, Example
{
	public ExampleConcrete() { }
	public override void Load()
	    => ((Example)this).OnLoad();
	public override void Run()
	    => ((Example)this).OnRun();
	[Binding]
	public String Value
	{
		get => Binding.Get(this).Open<String>(nameof(Value));
		set => Binding.Get(this).Place(nameof(Value), value);
	}

}