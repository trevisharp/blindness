using System;
using System.Threading;

namespace Blindness.Elements;

using Parallelism;

public class EventElement : IAsyncElement
{
    bool value;

    bool isRunning;
    AutoResetEvent signal;
    
    Action<bool> action;
    Func<bool> predicate;
    
    public EventElement(
        Func<bool> predicate,
        Action<bool> action
    )
    {
        this.isRunning = false;
        this.signal = new AutoResetEvent(false);    
        this.predicate = predicate;
        this.action = action;
    }

    public void Awake()
        => signal.Set();

    public void Await()
        => signal.WaitOne();

    public void Finish()
    {
        isRunning = false;
        signal.Set();
    }

    public void Start()
    {
        isRunning = true;
        value = predicate();
        action(value);

        while (isRunning)
        {
            signal.WaitOne();

            var newValue = predicate();
            if (newValue == value)
                continue;
            
            value = newValue;
            action(value);
        }
    }
}