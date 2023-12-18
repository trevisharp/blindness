namespace Blindness.Parallelism;

public interface IAsyncModel
{
    void Start();
    void Run(INode node);
    void Stop();
}