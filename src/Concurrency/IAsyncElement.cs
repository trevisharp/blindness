namespace Blindness.Concurrency;

public interface IAsyncElement
{
    void Start();
    void Await();
    void Finish();
}