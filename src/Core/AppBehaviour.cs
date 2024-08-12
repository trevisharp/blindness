/* Author:  Leonardo Trevisan Silio
 * Date:    12/08/2024
 */
using System;
namespace Blindness.Core;

/// <summary>
/// Represents a Blindess application.
/// </summary>
public abstract class AppBehaviour
{
    /// <summary>
    /// Get the executing INode.
    /// </summary>
    public abstract INode CurrentMainNode { get; }

    /// <summary>
    /// Run the Behaviour structures.
    /// Open a first Run in a app named 'main'.
    /// </summary>
    public abstract void Run<T>(params object[] parameters) where T : INode;

    /// <summary>
    /// Open or replace the current INode to run in app.
    /// </summary>
    public abstract void Open<T>(params object[] parameters) where T : INode;

    /// <summary>
    /// Remove all the INodes to execution stack.
    /// </summary>
    public abstract void Clear();

    /// <summary>
    /// Push a new INode to top of execution stack.
    /// </summary>
    public abstract void Push<T>(params object[] parameters) where T : INode;

    /// <summary>
    /// Pop the top of the execution stack.
    /// </summary>
    public abstract INode Pop();

    /// <summary>
    /// Create another parallel execution stack.
    /// </summary>
    public abstract void Create(string app);

    /// <summary>
    /// Start to use the another parallel execution stack.
    /// </summary>
    public abstract void MoveTo(string app);

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