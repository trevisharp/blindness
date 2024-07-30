/* Author:  Leonardo Trevisan Silio
 * Date:    30/07/2024
 */
using System;
using System.Threading;

namespace Blindness.Bind;

using Concurrency;

/// <summary>
/// Represents a event used to listen memory addresses.
/// </summary>
public class EventElement(
    IAsyncModel model,
    Action<EventArgs, bool> action,
    Func<EventArgs, bool> predicate,
    EventArgs eventState = null
    ) : BaseAsyncElement(model)
{
    readonly EventArgs state = eventState ?? EventArgs.Empty;
    readonly AutoResetEvent pauseSignal = new(false);
    bool paused = false;
    bool running = false;
    bool value = predicate(eventState ?? EventArgs.Empty);
    
    public override void Pause()
        => paused = true;

    public override void Resume()
    {
        paused = false;
        pauseSignal.Set();
    }

    public override void Run()
    {
        running = true;        
        PauseAndWaitResume();

        while (running)
        {
            var newValue = predicate(state);
            if (newValue == value)
            {
                value = newValue;
                action(state, value);
            }
            
            SendSignal(newValue == value);
            
            PauseAndWaitResume();
        }
    }

    public override void Stop()
    {
        running = false;
        Resume();
    }

    void PauseAndWaitResume()
    {
        Pause();
        if (paused)
            pauseSignal.WaitOne();
    }
}