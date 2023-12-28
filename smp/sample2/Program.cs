using System;

using Blindness;

Verbose.VerboseLevel = 10000;
App.StartWith<C1>();

public interface C1 : INode
{
    public C2 MyField { get; set; }

    void Deps(C2 MyField);

    void OnLoad()
    {
        Console.WriteLine("Olá, mundo!");
    }

    void OnRun()
    {
        Console.ReadKey(true);
        Console.WriteLine("Running...");
    }
}

public interface C2 : INode
{

}