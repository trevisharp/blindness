/* Author:  Leonardo Trevisan Silio
 * Date:    11/07/2024
 */
using System;
using System.Threading;

namespace Blindness.Concurrency.Elements;

using States;
using Internal;
using Exceptions;

/// <summary>
/// Loop a specific node.
/// </summary>
public class LoopNodeAppElement<T> : IAsyncElement
{
    public IAsyncModel Model { get; set; }
    public event Action<IAsyncElement, SignalArgs> OnSignal;
    public int ElementPointer { get; set; }

    bool running = false;
    AutoResetEvent signal = new(false);

    public void Wait()
        => signal.WaitOne();

    public void Stop()
        => running = false;

    public void Start()
    {
        var app = DependencySystem
            .Current.GetConcrete(typeof(T));
        this.ElementPointer = Memory.Current.Add(app);

        running = true;
        while (running)
        {
            var data = Memory.Current.GetObject(ElementPointer);
            if (data is IAsyncElement element)
            {
                element.Start();
                continue;
            }
            
            throw new NonAsyncElementException(typeof(T));
        }
    }
}