using System;
using System.Collections.Generic;

using Blindness;

var app = Root.New<MyApp>();
app.Run();

public class BaseComponent : Node { }

public class MyApp : Root
{
    protected virtual TableComponent table { get; set; }
    protected virtual InputComponent input { get; set; }

    protected override void OnLoad()
    {
        table |= size => 6;
        table |= texts => new List<string> {
            "Textos",
            "Salvos"
        };
        table |= _input => input;
    }

    protected override void OnProcess()
    {
        Console.Clear();
        table?.Process();
        input?.Process();
        Console.ReadKey(true);
    }
}

public class TableComponent : BaseComponent
{
    protected virtual int size { get; set; }
    protected virtual List<string> texts { get; set; }
    protected virtual InputComponent input { get; set; }
    protected virtual InputItemComponent itemInput { get; set; }
    protected virtual InputCommandComponent commandInput { get; set; }

    protected override void OnLoad()
    {
        input = itemInput;
        input |= n => size;
        input |= list => texts;
    }

    protected override void OnProcess()
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
        
        if (texts.Count == 10)
        {
            input = commandInput;
            input |= n => size;
            input |= list => texts;
        }
    }
}

public class InputComponent : BaseComponent
{
    protected virtual int n { get; set; }
    protected virtual List<string> list { get; set; }
}

public class InputItemComponent : InputComponent
{
    protected override void OnProcess()
    {
        Console.Write("Item a adicionar: ");
        var text = Console.ReadLine();
        
        if (list is null)
            return;
        
        list.Add(text);

        if (text.Length > n)
            n = text.Length;
    }
}

public class InputCommandComponent : InputComponent
{
    protected override void OnProcess()
    {
        Console.Write("Comando a rodar: ");
        var text = Console.ReadLine();
        
        if (list is null)
            return;
        
        if (text == "!clear")
        {
            list = new();
            return;
        }
    }
}