/* Author:  Leonardo Trevisan Silio
 * Date:    11/07/2024
 */
using System;
using System.Threading;

namespace Blindness.Concurrency.Elements;

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
            signal.WaitOne();

            var newValue = predicate();
            if (newValue == value)
                continue;
            
            value = newValue;
            action(value);
        }
    }
}