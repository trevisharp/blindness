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
    /// <summary>
    /// Get if the Run method already been called. The IsRunning
    /// property returns true even if the element is paused.
    /// </summary>
    public bool IsRunning { get; private set; } = false;

    /// <summary>
    /// Get if the element is paused and is not calling OnRun
    /// method.
    /// </summary>
    public bool IsPaused { get; private set; } = false;

    /// <summary>
    /// Get if the pause already is request but the pause
    /// has not yet been applied.
    /// </summary>
    public bool IsWaitingToPause { get; private set; } = false;

    /// <summary>
    /// Get if the resume already is request but the resume
    /// has not yet been applied.
    /// </summary>
    public bool IsWaitingToResume { get; private set; } = false;

    /// <summary>
    /// Get if the stop already is request but the stop
    /// has not yet been applied.
    /// </summary>
    public bool IsWaitingToStop { get; private set; } = false;
    readonly AutoResetEvent pauseSignal = new(false);

    public override void Pause()
    {
        if (IsPaused)
            return;
        
        IsWaitingToPause = true;
    }

    public override void Resume()
    {
        if (!IsPaused)
            return;

        pauseSignal.Set();
        IsWaitingToResume = true;
    }

    public override void Run()
    {
        OnInit();
        IsRunning = true;
        while (!IsWaitingToStop)
        {
            if (IsWaitingToPause)
            {
                IsPaused = true;
                IsWaitingToPause = false;
                pauseSignal.WaitOne();
                IsWaitingToResume = false;
                IsPaused = false;
            }

            OnRun();
        }
        IsRunning = false;
        OnStop();
    }

    public override void Stop()
        => IsWaitingToStop = true;
    
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