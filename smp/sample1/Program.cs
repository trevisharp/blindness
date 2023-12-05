using System;
using System.Text;
using System.Collections.Generic;

using Blindness;
using System.Runtime.InteropServices;
using System.Linq;

var app = DependencySystem
    .Current.GetConcrete(typeof(LoginScreen));
while (true)
    app.Process();

public interface LoginScreen : INode
{
    Panel Panel { get; set; }
    TextBox Login { get; set; }
    TextBox Password { get; set; }
    TextBox Repeat { get; set; }
    string login { get; set; }
    string password { get; set; }
    string repeat { get; set; }
    int selectedField { get; set; }

    void Deps(
        Panel Panel, 
        TextBox Login, 
        TextBox Password,
        TextBox Repeat
    );

    void OnLoad()
    {
        Panel.Title = "Login Page";
        Panel.Width = 60;
        Panel.Children = new List<INode>
        {
            Login, Password, Repeat
        };

        Login.Title = "login";
        Login.Size = 40;

        Password.Title = "password";
        Password.Size = 40;

        Repeat.Title = "repeat password";
        Repeat.Size = 40;

        Bind |= login => Login.Text;
        Bind |= password => Password.Text;
        Bind |= repeat => Repeat.Text;
    }

    void OnProcess()
    {
        Console.Clear();
        Panel.Process();

        var newChar = Console.ReadKey(true);
        if (newChar.Key == ConsoleKey.Tab)
        {
            selectedField =
                selectedField == 2 ? 0 :
                selectedField + 1;
            return;
        }
        
        switch (selectedField)
        {
            case 0:
                login += newChar.KeyChar;
                break;
            
            case 1:
                password += newChar.KeyChar;
                break;
            
            case 2:
                repeat += newChar.KeyChar;
                break;
        }
    }
}

public interface Panel : INode
{
    string Title { get; set; }
    int Width { get; set; }
    List<INode> Children { get; set; }

    void OnProcess()
    {
        StringBuilder sb = new StringBuilder();

        sb.Append("─ ");
        sb.Append(Title);
        sb.Append(" ");
        sb.Append('─', Width - Title.Length - 2);
        Console.WriteLine(sb);

        foreach (var child in Children)
            child.Process();
        
        sb.Clear();
        sb.Append('─', Width);
        Console.WriteLine(sb);
    }
}

public interface TextBox : INode
{
    string Title { get; set; }
    string Text { get; set; }
    int Size { get; set; }
    bool Selected { get; set; }

    void OnProcess()
    {
        StringBuilder sb = new StringBuilder();

        if (Selected)
        {
            sb.Append("╔");
            sb.Append(Title);
            sb.Append('═', Size + 2 - Title.Length);
            sb.AppendLine("╗");

            sb.Append("║ ");
            sb.Append(Text);
            sb.Append(' ', Size + 1 - Text.Length);
            sb.AppendLine("║");
        
            sb.Append("╚");
            sb.Append('═', Size + 2);
            sb.AppendLine("╝");
        }
        else
        {
            sb.Append("┌");
            sb.Append(Title);
            sb.Append('─', Size + 2 - Title.Length);
            sb.AppendLine("┐");

            sb.Append("│ ");
            sb.Append(Text);
            sb.Append(' ', Size + 1 - Text.Length);
            sb.AppendLine("│");
        
            sb.Append("└");
            sb.Append('─', Size + 2);
            sb.AppendLine("┘");

        }

        Console.WriteLine(sb);
    }
}

public interface Condition : INode
{
    int ValueExpected { get; set; }
    int RealValue { get; set; }
    bool Result { get; set; }

    void OnProcess()
    {
        Result = ValueExpected == RealValue;
    }
}