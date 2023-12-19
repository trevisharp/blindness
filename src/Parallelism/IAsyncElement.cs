namespace Blindness.Parallelism;

public interface IAsyncElement
{
    void Start();
    void Await();
    void Finish();
}