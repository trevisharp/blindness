using System.Threading;

namespace Blindness.Concurrency.Elements;

using States;
using Exceptions;

public class RealTimeElement<T> : IAsyncElement
{
    public IAsyncModel Model { get; set; }
    public int ElementPointer { get; set; }

    bool running = false;
    AutoResetEvent signal = new(false);

    public void Await()
        => signal.WaitOne();

    public void Finish()
        => running = false;

    public void Start()
    {
        running = true;
        while (running)
        {
            var data = Memory.Current.GetObject(ElementPointer);
            if (data is IAsyncElement element)
            {
                Model.Run(element);
                element.Await();
                continue;
            }
            
            throw new NonAsyncElementException(typeof(T));
        }
    }
}