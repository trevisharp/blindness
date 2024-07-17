/* Author:  Leonardo Trevisan Silio
 * Date:    11/07/2024
 */
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

    public override void Stop()
        => running = false;

    public override void Run()
    {
        var app = Node.New(typeof(T), Model);
        ElementPointer = app.MemoryLocation;

        running = true;
        while (running)
        {
            var data = Memory.Current.GetObject(ElementPointer);
            if (data is not IAsyncElement element)
                throw new NonAsyncElementException(typeof(T));
                
            element.Run();
            SendSignal(SignalArgs.True);
        }
    }
}