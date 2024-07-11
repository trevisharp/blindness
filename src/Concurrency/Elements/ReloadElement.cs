/* Author:  Leonardo Trevisan Silio
 * Date:    11/07/2024
 */
using System;
using System.Threading;

namespace Blindness.Concurrency.Elements;

/// <summary>
/// Run a first header and restart the second element every time
/// that first element make a step operation.
/// </summary>
public class ReloadElement(
    IAsyncModel model,
    IAsyncElement header,
    IAsyncElement follower
    ) : IAsyncElement
{
    private AutoResetEvent signal = new(false);

    public IAsyncModel Model => model;
    public IAsyncElement Header => header;
    public IAsyncElement Follower => follower;
    public event Action<IAsyncElement, SignalArgs> OnSignal;

    public void Wait() { }

    public void Finish()
        => signal.Set();

    public void Start()
    {
        Model.OnError += (el, ex) =>
        {
            if (el != Follower)
                return;
            
            Header.Wait();
            Model.Run(Follower);
        };

        Model.Run(Header);
        Model.Run(Follower);
    }
}