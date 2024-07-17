/* Author:  Leonardo Trevisan Silio
 * Date:    11/07/2024
 */
using System;
using System.Threading;

namespace Blindness.Concurrency.Elements;

/// <summary>
/// Run a first header and restart the second element every time
/// that first element make a step operation.
/// </summary>
public class ReloadElement(
        IAsyncModel model,
        IAsyncElement reloader,
        IAsyncElement main
    ) : BaseAsyncElement(model)
{
    bool paused = false;

    public IAsyncElement Reloader => reloader;
    public IAsyncElement Main => main;
    
    public override void Stop()
    {
        Model.OnError -= OnModelError;
        Reloader.OnSignal -= OnReloaderSignal;
    }

    public override void Run()
    {
        Model.OnError += OnModelError;
        Reloader.OnSignal += OnReloaderSignal;

        Model.Run(Reloader);
        Model.Run(Main);
    }

    /// <summary>
    /// Reset Main Async Element.
    /// </summary>
    public void ResetMain()
    {
        while (paused)
            Thread.Sleep(500);
        
        Main.Pause();
        Reloader.Wait();
        Main.Resume();
        SendSignal(SignalArgs.True);
    }

    void OnModelError(IAsyncElement el, Exception ex)
    {
        if (el != Main)
            return;
        
        ResetMain();
    }

    void OnReloaderSignal(IAsyncElement e, SignalArgs s)
    {
        if (s == SignalArgs.False)
            return;
        
        ResetMain();
    }

    public override void Pause()
        => paused = true;

    public override void Resume()
        => paused = false;
}