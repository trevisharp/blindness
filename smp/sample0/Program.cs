using System;
using System.Collections.Generic;

using Blindness;

var app = Root.New<MyApp>();

while (true)
{
    Console.Clear();
    app.Run();
    Console.WriteLine("Aperte qualquer botão pra proceder...");
    Console.ReadKey(true);
}

public class MyApp : Root
{
    protected virtual MyComponentA compA { get; set; }
    protected virtual MyComponentB compB { get; set; }

    protected override void Load()
    {
        compA |= size => 6;
        compA |= texts => new List<string> {
            "Textos",
            "Salvos"
        };
        compA |= compB => compB;
    }

    public void Run()
    {
        compA?.Run();
        compB?.Run();
    }
}

public class MyComponentA : Node<MyComponentA>
{
    protected virtual MyComponentB compB { get; set; }
    protected virtual int size { get; set; }
    protected virtual List<string> texts { get; set; }

    protected override void Load()
    {
        compB |= n => size;
        compB |= list => texts;
    }

    public void Run()
    {
        Console.Write("┌");
        for (int i = 0; i < size + 2; i++)
            Console.Write("─");
        Console.WriteLine("┐");

        int j = 0;
        foreach (var text in texts)
        {
            j++;
            
            Console.Write("│");
            Console.Write(" ");
            Console.Write(text);
            for (int i = text.Length; i < size + 1; i++)
                Console.Write(" ");
            Console.WriteLine("│");

            if (j == texts.Count)
            {
                Console.Write("└");
                for (int i = 0; i < size + 2; i++)
                    Console.Write("─");
                Console.WriteLine("┘");
                continue;
            }

            Console.Write("├");
            for (int i = 0; i < size + 2; i++)
                Console.Write("─");
            Console.WriteLine("┤");
        }
        
        if (texts.Count == 0)
            Console.WriteLine("\tLista vazia!");
        
        if (size == 10)
            compB = null;
    }
}

public class MyComponentB : Node<MyComponentB>
{
    protected virtual int n { get; set; }
    protected virtual List<string> list { get; set; }

    public void Run()
    {
        Console.Write("Item a adicionar: ");
        var text = Console.ReadLine();
        
        if (list is null)
            return;
        
        if (text == "!clear")
        {
            list = new();
            return;
        }
        
        list.Add(text);

        if (text.Length > n)
            n = text.Length;
    }
}