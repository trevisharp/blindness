using System;
using System.Collections.Generic;

using Blindness;

var component = new MyComponentA();

while (true)
{
    Console.Clear();
    component.Run();
    Console.WriteLine("Aperte qualquer botão pra proceder...");
    Console.ReadKey(true);
}

public class MyComponentA : Stateness
{
    int n = 0;
    List<string> texts = new();

    public void Run()
    {
        Console.WriteLine($"Máximo de caractéres: {n}");
        Console.WriteLine("Textos salvos:");
        foreach (var text in texts)
            Console.WriteLine($"\t-{text}");
        
        if (texts.Count == 0)
            Console.WriteLine("\tLista vazia!");
        
        new MyComponentB(n, texts).Run();
    }
}

public class MyComponentB : Stateness
{
    int n = default;
    List<string> texts = default;
    public MyComponentB(int n, List<string> texts)
        : base(texts) { }

    public void Run()
    {
        Console.Write("Item a adicionar: ");
        var text = Console.ReadLine();
        
        if (texts is null)
            return;
        
        texts.Add(text);

        if (text.Length > n)
            n = text.Length;
    }
}