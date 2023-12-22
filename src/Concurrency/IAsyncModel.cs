namespace Blindness.Concurrency;

public interface IAsyncModel
{
    void Start();
    void Run(IAsyncElement node);
    void Stop();
}