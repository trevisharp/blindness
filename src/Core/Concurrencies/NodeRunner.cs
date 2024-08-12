/* Author:  Leonardo Trevisan Silio
 * Date:    12/08/2024
 */
using System;
using System.Threading;

namespace Blindness.Core.Concurrencies;

using Concurrency;

/// <summary>
/// The AsyncElement that loop a Node element.
/// </summary>
public class NodeRunner(IAsyncModel model, Func<INode> NodeGetter) : BaseAsyncElement(model)
{
    bool running = false;
    bool paused = false;

    public override void Stop()
        => running = false;

    public override void Run()
    {
        running = true;
        while (running)
        {
            while (paused)
                Thread.Sleep(500);
            
            var currentNode = NodeGetter();
            if (currentNode is null)
                return;
            
            currentNode.Run();
        }
    }

    public override void Pause()
        => paused = true;

    public override void Resume()
        => paused = false;
}