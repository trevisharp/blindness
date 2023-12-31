/* Author:  Leonardo Trevisan Silio
 * Date:    01/01/2024
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
    bool isRunning = false;
    int activeCount = 0;
    AutoResetEvent stopSignal;
    AutoResetEvent queueSignal;
    ConcurrentQueue<IAsyncElement> queue = new();

    public event Action<IAsyncElement, Exception> OnError;

    public void Start()
    {
        this.isRunning = true;
        this.stopSignal = new(false);
        this.queueSignal = new(false);

        Task.Run(() => {
            while (isRunning)
            {
                if (queue.Count == 0)
                    queueSignal.WaitOne();
                
                bool dequeued = queue.TryDequeue(out IAsyncElement node);
                if (!dequeued)
                    continue;
                
                executeAsync(node);
            }
        });

        stopSignal.WaitOne();
    }

    public void Stop()
    {
        this.isRunning = false;
        stopSignal.Set();
    }

    public void Run(IAsyncElement node)
    {
        int coreCount = Environment.ProcessorCount;

        if (!isRunning || activeCount > 4 * coreCount)
        {
            queue.Enqueue(node);
            return;
        }

        if (activeCount < 2 * coreCount && !queue.IsEmpty)
            queueSignal.Set();
        
        executeAsync(node);
    }

    void executeAsync(IAsyncElement node)
    {
        Task.Run(() => {
            try
            {
                activeCount++;
                node.Start();
                activeCount--;
            }
            catch (Exception ex)
            {
                SendError(node, ex);
            }
        });
    }

    public void SendError(IAsyncElement el, Exception ex)
    {
        if (OnError is null)
            return;
        
        OnError(el, ex);
    }
}