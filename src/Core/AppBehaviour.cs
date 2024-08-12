/* Author:  Leonardo Trevisan Silio
 * Date:    12/08/2024
 */
using System;

namespace Blindness.Core;

/// <summary>
/// Represents a Blindess application
/// </summary>
public abstract class AppBehaviour
{
    /// <summary>
    /// Run the application.
    /// </summary>
    public abstract void Run<T>(bool debug);
    
    protected static void ShowError(Exception ex)
    {
        Verbose.Error(ex.Message, -1);

        var lines = ex.StackTrace.Split('\n');
        foreach (var line in lines)
        {
            var isInternal = line
                .Trim()
                .StartsWith("at Blindness");
            
            Verbose.Error(line, isInternal ? 1 : 0);
        }
    }
}