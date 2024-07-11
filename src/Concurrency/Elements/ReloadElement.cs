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
    public IAsyncElement Reloader => reloader;
    public IAsyncElement Main => main;

    readonly AutoResetEvent signal = new(false);
    
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
        Main.Stop();
        Reloader.Wait();
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
}