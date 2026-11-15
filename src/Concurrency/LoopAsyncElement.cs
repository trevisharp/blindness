/* Author:  Leonardo Trevisan Silio
 * Date:    18/07/2024
 */
using System.Threading;

namespace Blindness.Concurrency;

/// <summary>
/// Represents a basic async element with a loop in run function.
/// </summary>
public abstract class LoopAsyncElement(IAsyncModel model) : BaseAsyncElement(model)
{
    readonly object sync = new();
    readonly AutoResetEvent pauseSignal = new(false);
    volatile bool isRunning, isPaused, isWaitingToPause, isWaitingToResume, isWaitingToStop;

    /// <summary>
    /// Get if the Run method already been called. The IsRunning
    /// property returns true even if the element is paused.
    /// </summary>
    public bool IsRunning => isRunning;

    /// <summary>
    /// Get if the element is paused and is not calling OnRun
    /// method.
    /// </summary>
    public bool IsPaused => isPaused;

    /// <summary>
    /// Get if the pause already is request but the pause
    /// has not yet been applied.
    /// </summary>
    public bool IsWaitingToPause => isWaitingToPause;

    /// <summary>
    /// Get if the resume already is request but the resume
    /// has not yet been applied.
    /// </summary>
    public bool IsWaitingToResume => isWaitingToResume;

    /// <summary>
    /// Get if the stop already is request but the stop
    /// has not yet been applied.
    /// </summary>
    public bool IsWaitingToStop => isWaitingToStop;

    public override void Pause()
    {
        lock (sync)
        {
            if (isPaused)
                return;

            isWaitingToPause = true;
        }
    }

    public override void Resume()
    {
        lock (sync)
        {
            if (isWaitingToPause)
            {
                isWaitingToPause = false;
                return;
            }

            if (!isPaused)
                return;

            isWaitingToResume = true;
            pauseSignal.Set();
        }
    }

    public override void Run()
    {
        OnInit();
        isRunning = true;
        while (!isWaitingToStop)
        {
            bool pauseNow = false;
            lock (sync)
            {
                if (isWaitingToPause)
                {
                    isPaused = true;
                    isWaitingToPause = false;
                    pauseNow = true;
                }
            }

            if (pauseNow)
            {
                pauseSignal.WaitOne();
                lock (sync)
                {
                    isWaitingToResume = false;
                    isPaused = false;
                }
                continue;
            }

            OnRun();
        }
        isRunning = false;
        OnStop();
    }

    public override void Stop()
    {
        lock (sync)
        {
            isWaitingToStop = true;
            if (isPaused)
                pauseSignal.Set();
        }
    }

    /// <summary>
    /// Run this method when the Run method is called.
    /// </summary>
    protected abstract void OnInit();

    /// <summary>
    /// Run this method when the element is stoped. This may
    /// occur when Stop method is called, but can occur when
    /// the elements is selected by the model to run.
    /// </summary>
    protected abstract void OnStop();

    /// <summary>
    /// Run this method in every execution loop.
    /// </summary>
    protected abstract void OnRun();
}
