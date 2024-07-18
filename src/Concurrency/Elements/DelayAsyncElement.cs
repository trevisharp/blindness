/* Author:  Leonardo Trevisan Silio
 * Date:    18/07/2024
 */
using System;
using System.Threading;

namespace Blindness.Concurrency.Elements;

public class DelayAsyncElement(
    IAsyncModel model,
    double seconds,
    Action onFinish
    ) : BaseAsyncElement(model)
{
    bool isRunning = false;
    DateTime start = DateTime.MaxValue;
    DateTime pauseStart = DateTime.MaxValue;

    public override void Pause()
    {
        if (pauseStart != DateTime.MaxValue)
            return;

        pauseStart = DateTime.Now;
    }

    public override void Resume()
    {
        if (pauseStart == DateTime.MaxValue)
            return;
        
        var time = DateTime.Now - pauseStart;
        pauseStart = DateTime.MaxValue;
        seconds += time.TotalSeconds;
    }

    public override void Run()
    {
        start = DateTime.Now;
        isRunning = true;

        while (isRunning) {
            Thread.Sleep(25);
            
            if (pauseStart != DateTime.MaxValue)
                continue;

            var time = DateTime.Now - start;
            if (time.TotalSeconds > seconds)
                break;
        }

        if (isRunning)
            onFinish();
    }

    public override void Stop()
        => isRunning = false;
}