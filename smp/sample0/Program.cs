using System;
using System.Collections.Generic;
using System.Text;
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
        Console.Clear();
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

        Bind |= size => itemInput.n;
        Bind |= texts => itemInput.list;

        Bind |= size => commandInput.n;
        Bind |= texts => commandInput.list;
    }

    void OnProcess()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("┌");
        sb.Append('─', size + 2);
        sb.AppendLine("┐");

        int j = 0;
        foreach (var text in texts)
        {
            j++;
            
            sb.Append("│ ");
            sb.Append(text);
            sb.Append(' ', size + 1 - text.Length);
            sb.AppendLine("│");

            if (j == texts.Count)
            {
                sb.Append("└");
                sb.Append('─', size + 2);
                sb.AppendLine("┘");
                continue;
            }

            
            sb.Append("├");
            sb.Append('─', size + 2);
            sb.AppendLine("┤");
        }
        
        if (texts.Count == 0)
        {
            sb.Append("└");
            sb.Append('─', size + 2);
            sb.AppendLine("┘");
        }
        Console.WriteLine(sb);
        
        input =
            texts.Count == 10 ?
            commandInput :
            itemInput;
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