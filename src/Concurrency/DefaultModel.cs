/* Author:  Leonardo Trevisan Silio
 * Date:    11/07/2024
 */
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace Blindness.Concurrency;

/// <summary>
/// Default Model used in application.
/// </summary>
public class DefaultModel : IAsyncModel
{
    volatile bool isRunning = false;
    int activeCount = 0;
    readonly AutoResetEvent queueSignal = new(false);
    readonly ConcurrentQueue<IAsyncElement> queue = new();

    public event Action<IAsyncElement, Exception> OnError;

    /// <summary>
    /// The number of Elements running by Core to wait run a new Element.
    /// </summary>
    public int HighPressureLimit { get; set; } = 4;

    bool IsHighPressure
        => Volatile.Read(ref activeCount) > HighPressureLimit * Environment.ProcessorCount;

    public void Start()
    {
        isRunning = true;

        while (isRunning)
        {
            var emptyQueue = queue.IsEmpty;
            var inactive = Volatile.Read(ref activeCount) == 0;

            if (emptyQueue && inactive)
                break;

            if (emptyQueue || IsHighPressure)
                queueSignal.WaitOne();

            if (!queue.TryDequeue(out IAsyncElement node))
                continue;

            ExecuteAsync(node);
        }

        isRunning = false;
    }

    /// <summary>
    /// Stop the model loop without interrupt running elements.
    /// </summary>
    public void Stop()
    {
        isRunning = false;
        queueSignal.Set();
    }

    public void Run(IAsyncElement node)
    {
        ArgumentNullException.ThrowIfNull(node, nameof(node));

        if (!isRunning || IsHighPressure)
        {
            queue.Enqueue(node);
            queueSignal.Set();
            return;
        }

        ExecuteAsync(node);
    }

    public void SendError(IAsyncElement el, Exception ex)
        => OnError?.Invoke(el, ex);

    void ExecuteAsync(IAsyncElement node)
    {
        if (node is null)
            return;

        Interlocked.Increment(ref activeCount);
        Task.Run(() => {
            try
            {
                node.Run();
            }
            catch (Exception ex)
            {
                SendError(node, ex);
            }
            finally
            {
                Interlocked.Decrement(ref activeCount);
                queueSignal.Set();
            }
        });
    }
}
