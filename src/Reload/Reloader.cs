/* Author:  Leonardo Trevisan Silio
 * Date:    12/08/2024
 */
using System;
using System.Reflection;

namespace Blindness.Reload;

/// <summary>
/// A Assembly reloader that get a new assembly when files changed.
/// </summary>
public abstract class Reloader
{
    public FileWatcher Watcher { get; set; }
    public AssemblyCompiler Compiler { get; set; }

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
        if (OnReload is null)
            return;
        if (Watcher is null)
            return;
        if (Compiler is null)
            return;
        
        var verify = Watcher.Verify();
        if (!verify)
            return;
        
        var newAssembly = Compiler.Get();
        if (newAssembly is null)
            return;
        
        OnReload(newAssembly);
    }
}