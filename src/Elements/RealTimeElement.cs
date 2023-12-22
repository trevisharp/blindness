using System.Threading;

namespace Blindness.Elements;

using Concurrency;

public class RealTimeElement : IAsyncElement
{
    public IAsyncModel Model { get; set; }
    public IAsyncElement Element { get; set; }

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
            Model.Run(Element);
            Element.Await();
        }
    }
}