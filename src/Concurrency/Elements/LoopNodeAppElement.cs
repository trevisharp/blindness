/* Author:  Leonardo Trevisan Silio
 * Date:    17/07/2024
 */
using System.Threading;

namespace Blindness.Concurrency.Elements;

using States;
using Exceptions;

/// <summary>
/// Loop a specific node.
/// </summary>
public class LoopNodeAppElement<T>(IAsyncModel model) : BaseAsyncElement(model)
{
    public int ElementPointer { get; set; }
    bool running = false;
    bool paused = false;

    public override void Stop()
        => running = false;

    public override void Run()
    {
        var app = Node.New(typeof(T), Model);
        ElementPointer = app.MemoryLocation;

        running = true;
        while (running)
        {
            while (paused)
                Thread.Sleep(500);
            
            var data = Memory.Current.GetObject(ElementPointer);
            if (data is not IAsyncElement element)
                throw new NonAsyncElementException(typeof(T));
                
            element.Run();
            SendSignal(SignalArgs.True);
        }
    }

    public override void Pause()
        => paused = true;

    public override void Resume()
        => paused = false;
}