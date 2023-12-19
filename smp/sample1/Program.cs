using System;
using System.Text;
using System.Collections.Generic;

using Blindness;

Verbose.VerboseLevel = 1000;
App.StartWith<LoginScreen>();

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
        Panel.Children = new() {
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

        On(
            () => selectedField == 0,
            () => Login.Selected = true
        );

        On(
            () => selectedField == 1,
            () => Password.Selected = true
        );

        On(
            () => selectedField == 2,
            () => Repeat.Selected = true
        );

        // Login.Bind |= Selected => selectedField == 0;
    }

    void OnProcess()
    {
        Console.Clear();
        Panel.Start();

        var newChar = Console.ReadKey(true);
        if (newChar.Key == ConsoleKey.Tab)
        {
            selectedField =
                selectedField == 2 ? 0 :
                selectedField + 1;
            return;
        }

        if (newChar.Key == ConsoleKey.Backspace)
        {
            switch (selectedField)
            {
                case 0:
                    if (login.Length == 0)
                        break;
                    login = login
                        .Substring(0, login.Length - 1);
                    break;
                
                case 1:
                    if (password.Length == 0)
                        break;
                    password = password
                        .Substring(0, password.Length - 1);
                    break;
                
                case 2:
                    if (repeat.Length == 0)
                        break;
                    repeat = repeat
                        .Substring(0, repeat.Length - 1);
                    break;
            }
            return;
        }

        if (newChar.Key == ConsoleKey.Enter)
            return;
        
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
            child.Start();
        
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
        int size = 8 * Size / 10;
        var text = Text.Length < size ? Text : 
            Text.Substring(
            Text.Length - size, size
        );

        if (Selected)
        {
            sb.Append("╔");
            sb.Append(Title);
            sb.Append('═', Size + 2 - Title.Length);
            sb.AppendLine("╗");

            sb.Append("║ ");
            sb.Append(text);
            sb.Append(' ', Size + 1 - text.Length);
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
            sb.Append(text);
            sb.Append(' ', Size + 1 - text.Length);
            sb.AppendLine("│");
        
            sb.Append("└");
            sb.Append('─', Size + 2);
            sb.AppendLine("┘");
        }

        Console.WriteLine(sb);
    }
}