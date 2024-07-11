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
    ) : IAsyncElement
{
    public IAsyncModel Model => model;
    public IAsyncElement Reloader => reloader;
    public IAsyncElement Main => main;
    public event Action<IAsyncElement, SignalArgs> OnSignal;

    readonly AutoResetEvent signal = new(false);

    public void Wait()
        => signal.WaitOne();
    
    public void Stop()
    {
        Model.OnError -= OnModelError;
        Reloader.OnSignal -= OnReloaderSignal;
    }

    public void Start()
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
    }

    void OnModelError(IAsyncElement el, Exception ex)
    {
        if (el != Main)
            return;
        
        Main.Stop();
    }

    void SendSignal()
    {
        signal.Set();
        if (OnSignal is not null)
            OnSignal(this, SignalArgs.True);
    }

    void OnReloaderSignal(IAsyncElement e, SignalArgs s)
    {
        if (s == SignalArgs.False)
            return;
        
        SendSignal();
        ResetMain();
    }
}