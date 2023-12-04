using System;
using System.Collections.Generic;

using Blindness;

var app = DependencySystem
    .Current.GetConcrete(typeof(MyApp));
while (true)
    app.Process();

public interface MyApp : INode
{
    TableComponent table { get; set; }
    InputComponent input { get; set; }

    void Deps(TableComponent table);

    void OnLoad()
    {
        table.size = 6;
        table.texts = new List<string> {
            "Textos",
            "Salvos"
        };
        table.Bind |= input => this.input;
    }

    void OnProcess()
    {
        // Console.Clear();
        table?.Process();
        input?.Process();
        Console.ReadKey(true);
    }
}

public interface TableComponent : INode
{
    int size { get; set; }
    List<string> texts { get; set; }
    InputComponent input { get; set; }
    InputItemComponent itemInput { get; set; }
    InputCommandComponent commandInput { get; set; }

    void Deps(
        InputItemComponent itemInput,
        InputCommandComponent commandInput
    );

    void OnLoad()
    {
        input = itemInput;
        Bind |= size => input.n;
        Bind |= texts => input.list;
    }

    void OnProcess()
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
            Bind |= size => input.n;
            Bind |= texts => input.list;
        }
    }
}

public interface InputComponent : INode
{
    int n { get; set; }
    List<string> list { get; set; }
}

public interface InputItemComponent : InputComponent
{
    void OnProcess()
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

public interface InputCommandComponent : InputComponent
{
    void OnProcess()
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