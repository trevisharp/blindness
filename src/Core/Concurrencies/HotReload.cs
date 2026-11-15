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
    readonly ManualResetEvent stopSignal = new(false);
    volatile bool running = false;
    volatile bool paused = false;

    /// <summary>
    /// The interval between two checks for file changes.
    /// </summary>
    public TimeSpan PollingInterval { get; set; } = TimeSpan.FromMilliseconds(100);

    public override void Stop()
    {
        running = false;
        stopSignal.Set();
    }

    public override void Run()
    {
        running = true;
        stopSignal.Reset();
        while (running)
        {
            if (!paused)
                reloader.TryReload();
            
            stopSignal.WaitOne(paused ? TimeSpan.FromMilliseconds(500) : PollingInterval);
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