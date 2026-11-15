/* Author:  Leonardo Trevisan Silio
 * Date:    01/01/2024
 */
using System;
using System.Collections.Generic;

using Color = System.ConsoleColor;
using static System.Console;

namespace Blindness;

/// <summary>
/// Class to Verbose messages with Verbole level in Console
/// </summary>
public static class Verbose
{
    public static int VerboseLevel = 0;
    private static string tabInfo = null;
    private static readonly object sync = new();

    /// <summary>
    /// The destinations of the messages. The console is the default one.
    /// </summary>
    public static List<Action<VerboseMessage>> Sinks { get; } = [ ConsoleSink ];

    /// <summary>
    /// Write the messages on console, with colors. Errors go to the standard error.
    /// </summary>
    public static void ConsoleSink(VerboseMessage message)
    {
        var writer = message.Kind == VerboseKind.Error ? Console.Error : Console.Out;
        if (message.Inline) writer.Write(" ");
        else writer.WriteLine();

        var original = ForegroundColor;
        ForegroundColor = message.Kind switch
        {
            VerboseKind.Info => Color.Blue,
            VerboseKind.Success => Color.Green,
            VerboseKind.Warning => Color.Yellow,
            VerboseKind.Error => Color.Red,
            _ => Color.White
        };
        writer.Write(message.Text);
        ForegroundColor = original;
    }

    /// <summary>
    /// Write the messages on System.Diagnostics.Debug, visible on the IDE output when there is no console.
    /// </summary>
    public static void DebugSink(VerboseMessage message)
    {
        if (message.Inline)
            System.Diagnostics.Debug.Write(" " + message.Text);
        else System.Diagnostics.Debug.WriteLine($"[{message.Kind}] {message.Text}");
    }

    private static void message(
        int level,
        object msg,
        VerboseKind kind,
        bool inline = false
    )
    {
        if (VerboseLevel < level)
            return;

        lock (sync)
        {
            var text = inline || msg is "" ? $"{msg}" : $"{tabInfo}{msg}";
            var verboseMessage = new VerboseMessage(kind, level, text, inline);
            foreach (var sink in Sinks)
                sink(verboseMessage);
        }
    }

    /// <summary>
    /// Create a group of messages with auto-tabulation
    /// </summary>
    public static void StartGroup(int level = 0)
    {
        if (VerboseLevel < level)
            return;

        if (tabInfo is null)
        {
            tabInfo = "";
            return;
        }

        tabInfo += "\t";
    }

    /// <summary>
    /// End a group of auto-tabulation
    /// </summary>
    public static void EndGroup(int level = 0)
    {
        if (VerboseLevel < level)
            return;
        
        if (tabInfo is null)
            return;
        NewLine(level);

        if (tabInfo == "")
        {
            tabInfo = null;
            return;
        }

        tabInfo = tabInfo[..^1];
    }
    
    /// <summary>
    /// Blue message to show information in console
    /// </summary>
    public static void Info(object text, int level = 0)
        => message(level, text, VerboseKind.Info);

    /// <summary>
    /// White message to show content in console
    /// </summary>
    public static void Content(object text, int level = 0)
        => message(level, text, VerboseKind.Content);
    
    /// <summary>
    /// Inline white message to show content in console
    /// </summary>
    public static void InlineContent(object text, int level = 0)
        => message(level, text, VerboseKind.Content, true);
    
    /// <summary>
    /// Green message to show success in console
    /// </summary>
    public static void Success(object text, int level = 0)
        => message(level, text, VerboseKind.Success);
    
    /// <summary>
    /// Yellow message to show warning in console
    /// </summary>
    public static void Warning(object text, int level = 0)
        => message(level, text, VerboseKind.Warning);
    
    /// <summary>
    /// Red message to show error in console
    /// </summary>
    public static void Error(object text, int level = 0)
        => message(level, text, VerboseKind.Error);
    
    /// <summary>
    /// Add new line in console
    /// </summary>
    public static void NewLine(int level = 0)
        => message(level, "", VerboseKind.Content);
}