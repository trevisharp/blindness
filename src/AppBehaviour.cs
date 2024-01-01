/* Author:  Leonardo Trevisan Silio
 * Date:    01/01/2024
 */
using System;

namespace Blindness;

/// <summary>
/// Represents a Blindess application
/// </summary>
public abstract class AppBehaviour
{
    /// <summary>
    /// Run the application.
    /// </summary>
    public abstract void Run<T>(bool debug)
        where T : INode;
    
    protected void showError(Exception ex)
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