using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace Blindness.Parallelism;

public class MainModel : IAsyncModel
{
    bool isRunning;
    int activeCount;
    AutoResetEvent stopSignal;
    AutoResetEvent queueSignal;
    ConcurrentQueue<INode> queue;

    public void Start()
    {
        this.activeCount = 0;
        this.isRunning = true;
        this.stopSignal = new(false);
        this.queueSignal = new(false);
        this.queue = new();

        Task.Run(() => {
            while (isRunning)
            {
                if (queue.Count == 0)
                    queueSignal.WaitOne();
                
                bool dequeued = queue.TryDequeue(out INode node);
                if (!dequeued)
                    continue;
            }
        });

        stopSignal.WaitOne();
    }

    public void Stop()
    {
        this.isRunning = false;
        stopSignal.Set();
    }

    public void Run(INode node)
    {
        int coreCount = Environment.ProcessorCount;

        if (!isRunning || activeCount > 4 * coreCount)
        {
            queue.Enqueue(node);
            return;
        }

        if (activeCount < 2 * coreCount && !queue.IsEmpty)
            queueSignal.Set();
        
        execute(node);
    }

    void execute(INode node)
    {
        Task.Run(() => {
            activeCount++;
            node.Process();
            activeCount--;
        });
    }
}