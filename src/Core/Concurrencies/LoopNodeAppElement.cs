/* Author:  Leonardo Trevisan Silio
 * Date:    18/07/2024
 */
using System.Threading;

namespace Blindness.Core.Concurrencies;

using Exceptions;
using Concurrency;

/// <summary>
/// Loop a specific node.
/// </summary>
public class LoopNodeAppElement<T>(IAsyncModel model) : BaseAsyncElement(model)
{
    bool running = false;
    bool paused = false;

    public override void Stop()
        => running = false;

    public override void Run()
    {
        // TODO: Apply New Box abstraction
        var app = Node.New(typeof(T));
        // ElementPointer = app.MemoryLocation;

        running = true;
        while (running)
        {
            while (paused)
                Thread.Sleep(500);
            
            // var data = Memory.Current.GetObject(ElementPointer);
            // if (data is not IAsyncElement element)
            //     throw new NonAsyncElementException(typeof(T));
                
            // element.Run();
            SendSignal(SignalArgs.True);
        }
    }

    public override void Pause()
        => paused = true;

    public override void Resume()
        => paused = false;
}