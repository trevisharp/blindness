using System;

namespace Blindness.Concurrency;

public interface IAsyncModel
{
    void Start();
    void Run(IAsyncElement node);
    void Stop();
    event Action<IAsyncElement, Exception> OnError;
    void SendError(IAsyncElement el, Exception ex);
}