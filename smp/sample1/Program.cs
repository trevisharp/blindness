using System;
using System.Text;
using System.Collections.Generic;

using Blindness.Core;
using System.Threading;
using Blindness;

Verbose.VerboseLevel = int.MaxValue;
App.Start<Example>();

public interface Example : INode
{
    string Value { get; set; }

    void OnLoad()
    {
        Value = "Hello, Blindness!";
        When(() => Random.Shared.Next(10) == 0, () => Value += "!");
    }

    void OnRun()
    {
        Console.Clear();
        Console.WriteLine(Value);
        Thread.Sleep(200);
    }
}