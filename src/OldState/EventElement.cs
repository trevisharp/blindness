/* Author:  Leonardo Trevisan Silio
 * Date:    18/07/2024
 */
using System;
using System.Threading;

namespace Blindness.State;

using Concurrency;

/// <summary>
/// Represents a event used to listen memory addresses.
/// </summary>
public class EventElement(
    IAsyncModel model,
    Action<bool> action,
    Func<bool> predicate
    ) : IAsyncElement
{
    bool value;
    bool paused = false;
    bool isRunning = false;
    readonly AutoResetEvent signal = new(false);

    public IAsyncModel Model => model;
    public event Action<IAsyncElement, SignalArgs> OnSignal;

    public void Awake()
        => signal.Set();

    public void Wait()
        => signal.WaitOne();

    public void Stop()
    {
        isRunning = false;
        signal.Set();
    }

    public void Run()
    {
        isRunning = true;
        value = predicate();
        action(value);

        while (isRunning)
        {
            while (paused)
                Thread.Sleep(100);

            signal.WaitOne();

            var newValue = predicate();
            if (newValue == value)
                continue;

            value = newValue;
            action(value);
            if (OnSignal is not null)
                OnSignal(this, SignalArgs.True);
        }
    }

    public void Pause()
        => paused = true;

    public void Resume()
        => paused = false;
}