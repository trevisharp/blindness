namespace Blindness.Parallelism;

public interface IAsyncModel
{
    void Start();
    void Run(IAsyncElement node);
    void Stop();
}