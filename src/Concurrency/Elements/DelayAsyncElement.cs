/* Author:  Leonardo Trevisan Silio
 * Date:    18/07/2024
 */
using System;
using System.Threading;
using System.Diagnostics;

namespace Blindness.Concurrency.Elements;

/// <summary>
/// Run a action after a delay. Pausing the element pauses the countdown.
/// </summary>
public class DelayAsyncElement(
    IAsyncModel model,
    double seconds,
    Action onFinish
    ) : BaseAsyncElement(model)
{
    readonly Stopwatch elapsed = new();
    readonly ManualResetEvent stopSignal = new(false);
    readonly object sync = new();
    volatile bool isRunning = false;

    public override void Pause()
    {
        lock (sync)
            elapsed.Stop();
    }

    public override void Resume()
    {
        lock (sync)
        {
            if (isRunning)
                elapsed.Start();
        }
    }

    public override void Run()
    {
        lock (sync)
        {
            isRunning = true;
            stopSignal.Reset();
            elapsed.Restart();
        }

        while (isRunning)
        {
            TimeSpan remaining;
            lock (sync)
                remaining = TimeSpan.FromSeconds(seconds) - elapsed.Elapsed;

            if (remaining <= TimeSpan.Zero)
                break;

            var wait = remaining < TimeSpan.FromMilliseconds(50) ? remaining : TimeSpan.FromMilliseconds(50);
            stopSignal.WaitOne(wait);
        }

        if (isRunning)
        {
            isRunning = false;
            onFinish();
        }
    }

    public override void Stop()
    {
        isRunning = false;
        stopSignal.Set();
    }
}
