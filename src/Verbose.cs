/* Author:  Leonardo Trevisan Silio
 * Date:    01/01/2024
 */
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
    private static void message(
        int level,
        object msg, 
        Color color,
        bool inline = false
    )
    {
        if (VerboseLevel < level)
            return;
        
        if (inline) Write(" ");
        else WriteLine();

        if (!inline)
            Write(tabInfo);
        
        ForegroundColor = color;
        Write(msg);
        ForegroundColor = Color.Gray;
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
        WriteLine();
        
        if (tabInfo == "")
        {
            tabInfo = null;
            return;
        }

        tabInfo = tabInfo.Remove(0);
    }
    
    /// <summary>
    /// Blue message to show information in console
    /// </summary>
    public static void Info(object text, int level = 0)
        => message(level, text, Color.Blue);

    /// <summary>
    /// White message to show content in console
    /// </summary>
    public static void Content(object text, int level = 0)
        => message(level, text, Color.White);
    
    /// <summary>
    /// Inline white message to show content in console
    /// </summary>
    public static void InlineContent(object text, int level = 0)
        => message(level, text, Color.White, true);
    
    /// <summary>
    /// Green message to show success in console
    /// </summary>
    public static void Success(object text, int level = 0)
        => message(level, text, Color.Green);
    
    /// <summary>
    /// Yellow message to show warning in console
    /// </summary>
    public static void Warning(object text, int level = 0)
        => message(level, text, Color.Yellow);
    
    /// <summary>
    /// Red message to show error in console
    /// </summary>
    public static void Error(object text, int level = 0)
        => message(level, text, Color.Red);
    
    /// <summary>
    /// Add new line in console
    /// </summary>
    public static void NewLine(int level = 0)
    {
        if (VerboseLevel < level)
            return;
        
        WriteLine();
    }
}