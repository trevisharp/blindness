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
    //─│┌┐└┘├┤
    int n = 6;
    List<string> texts = new() {
        "Textos",
        "Salvos"
    };
    MyComponentB compB = null;

    public void Run()
    {
        Console.Write("┌");
        for (int i = 0; i < n + 2; i++)
            Console.Write("─");
        Console.WriteLine("┐");

        int j = 0;
        foreach (var text in texts)
        {
            j++;
            
            Console.Write("│");
            Console.Write(" ");
            Console.Write(text);
            for (int i = text.Length; i < n + 1; i++)
                Console.Write(" ");
            Console.WriteLine("│");

            if (j == texts.Count)
            {
                Console.Write("└");
                for (int i = 0; i < n + 2; i++)
                    Console.Write("─");
                Console.WriteLine("┘");
                continue;
            }

            Console.Write("├");
            for (int i = 0; i < n + 2; i++)
                Console.Write("─");
            Console.WriteLine("┤");
        }
        
        if (texts.Count == 0)
            Console.WriteLine("\tLista vazia!");
        
        compB.Run();
    }
}

public class MyComponentB : Stateness
{
    int n = default;
    List<string> texts = default;
    public MyComponentB(int n, List<string> texts) { }

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