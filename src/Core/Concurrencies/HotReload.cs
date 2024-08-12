/* Author:  Leonardo Trevisan Silio
 * Date:    12/08/2024
 */
using System;
using System.Threading;

namespace Blindness.Core.Concurrencies;

using Reload;
using Concurrency;

/// <summary>
/// The Hot Reload System Async Element.
/// </summary>
public class HotReload(IAsyncModel model) : BaseAsyncElement(model)
{
    readonly Reloader reloader = Reloader.GetDefault();
    bool running = false;
    bool paused = false;

    public override void Stop()
        => running = false;

    public override void Run()
    {
        reloader.OnReload += assembly =>
        {
            SendSignal(new AssemblySignalArgs(assembly, true));
            reloader.Watcher.Reset();
        };
        
        running = true;
        while (running)
        {
            while (paused)
                Thread.Sleep(500);
            
            reloader.TryReload();
        }
    }

    public void AddAction(Action action)
        => reloader.Actions.Add(action);

    public override void Pause()
        => paused = true;

    public override void Resume()
        => paused = false;
}