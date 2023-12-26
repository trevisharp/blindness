using System;

using Blindness;

Verbose.VerboseLevel = 10000;
App.StartWith<C1>();

public interface C1 : INode
{

}

public interface C2 : C1
{

}