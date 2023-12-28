using System.Threading;

namespace Blindness.Concurrency.Elements;

public class ChainElement : IAsyncElement
{
    private AutoResetEvent signal = new(false);

    public IAsyncModel Model { get; set; }
    public IAsyncElement First { get; set; }
    public IAsyncElement Second { get; set; }

    public void Await() { }

    public void Finish()
        => signal.Set();

    public void Start()
    {
        Model.Run(First);
        First.Await();

        Model.Run(Second);

        signal.WaitOne();
    }
}