/* Author:  Leonardo Trevisan Silio
 * Date:    14/08/2024
 */
using System;
using System.Reflection;
using System.Collections.Generic;

namespace Blindness.Reload;

/// <summary>
/// A Assembly reloader that get a new assembly when files changed.
/// </summary>
public abstract class Reloader
{
    public FileWatcher Watcher { get; set; }
    public AssemblyCompiler Compiler { get; set; }

    /// <summary>
    /// Actions to run when files are modifieds.
    /// </summary>
    public List<Action> Actions { get; private set; } = [];

    /// <summary>
    /// Get the default configuration of Reloader class.
    /// </summary>
    public static Reloader GetDefault()
        => new DefaultReloader();

    public event Action<Assembly> OnReload;

    /// <summary>
    /// Try Reload code calling OnReload on success operation.
    /// </summary>
    public void TryReload()
    {
        if (OnReload is null || Watcher is null || Compiler is null)
            return;
        
        if (!Watcher.Verify())
            return;
        
        Reload();
    }

    /// <summary>
    /// Force Reload wihtout verification.
    /// </summary>
    public void Reload()
    {
        if (Compiler is null)
            return;

        foreach (var action in Actions)
        {
            if (action is null)
                continue;
            
            action();
        }
        
        var newAssembly = Compiler.Get();
        if (newAssembly is null)
            return;
        
        OnReload?.Invoke(newAssembly);
    }
}