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
    bool isRunning = false;
    int activeCount = 0;
    AutoResetEvent queueSignal;
    ConcurrentQueue<IAsyncElement> queue = new();

    public event Action<IAsyncElement, Exception> OnError;

    /// <summary>
    /// The number of Elements running by Core to wait run a new Element.
    /// </summary>
    public int HighPressureLimit { get; set; } = 4;

    public void Start()
    {
        isRunning = true;
        queueSignal = new(false);

        while (isRunning)
        {
            var pressureLimit = HighPressureLimit * Environment.ProcessorCount;
            var highPressure = activeCount > pressureLimit;
            var emptyQueue = queue.IsEmpty;
            var inactive = activeCount == 0;

            if (emptyQueue && inactive)
                break;

            if (emptyQueue || highPressure)
                queueSignal.WaitOne();
            
            if (!queue.TryDequeue(out IAsyncElement node))
                continue;
            
            ExecuteAsync(node);
        }
    }

    public void Stop()
        => isRunning = false;

    public void Run(IAsyncElement node)
    {
        if (node is null)
            throw new ArgumentNullException(nameof(node));
        
        var pressureLimit = HighPressureLimit * Environment.ProcessorCount;
        var highPressure = activeCount > pressureLimit;

        if (!isRunning || highPressure)
        {
            queue.Enqueue(node);
            return;
        }
        
        ExecuteAsync(node);
    }

    public void SendError(IAsyncElement el, Exception ex)
    {
        if (OnError is null)
            return;
        
        OnError(el, ex);
    }

    void ExecuteAsync(IAsyncElement node)
    {
        if (node is null)
            return;
        
        activeCount++;
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
                activeCount--;
                queueSignal.Set();
            }
        });
    }
}