/* Author:  Leonardo Trevisan Silio
 * Date:    11/07/2024
 */
using System;
using System.Threading;

namespace Blindness.Concurrency;

/// <summary>
/// A Base class to all Async Elements that simplify SendSignal action.
/// </summary>
public abstract class BaseAsyncElement(IAsyncModel model) : IAsyncElement
{
    public IAsyncModel Model => model;

    public event Action<IAsyncElement, SignalArgs> OnSignal;
    
    readonly AutoResetEvent signal = new(false);

    public abstract void Run();

    public abstract void Stop();

    public void Wait()
        => signal.WaitOne();

    protected void SendSignal(SignalArgs args)
    {
        signal.Set();
        if (OnSignal is not null)
            OnSignal(this, args);
    }
}