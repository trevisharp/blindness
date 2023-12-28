using System.Threading;

namespace Blindness.Concurrency.Elements;

public class ReloadLoopElement : IAsyncElement
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
        Model.OnError += (el, ex) =>
        {
            if (el != Second)
                return;
            
            First.Await();
            Model.Run(Second);
        };

        Model.Run(First);
        Model.Run(Second);
    }
}