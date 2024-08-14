/* Author:  Leonardo Trevisan Silio
 * Date:    14/08/2024
 */
using System;
using System.Threading;

namespace Blindness.Core.Concurrencies;

using Reload;
using Concurrency;

/// <summary>
/// The Hot Reload System Async Element.
/// </summary>
public class HotReload : BaseAsyncElement
{
    public HotReload(IAsyncModel model) : base(model)
    {
        reloader.OnReload += assembly =>
        {
            SendSignal(new AssemblySignalArgs(assembly, true));
            reloader.Watcher.Reset();
        };
    }

    readonly Reloader reloader = Reloader.GetDefault();
    bool running = false;
    bool paused = false;

    public override void Stop()
        => running = false;

    public override void Run()
    {
        running = true;
        while (running)
        {
            while (paused)
                Thread.Sleep(500);
            
            reloader.TryReload();
        }
    }

    public void Force()
        => reloader.Reload();

    public void AddAction(Action action)
        => reloader.Actions.Add(action);

    public override void Pause()
        => paused = true;

    public override void Resume()
        => paused = false;
}