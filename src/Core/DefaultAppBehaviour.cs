/* Author:  Leonardo Trevisan Silio
 * Date:    22/07/2024
 */
using System;

namespace Blindness.Core;

using States;
using Reload;
using Injection;
using Concurrency;

/// <summary>
/// The default structure to a Blindness app.
/// </summary>
public class DefaultAppBehaviour : AppBehaviour
{
    public override void Run<T>(bool debug)
    {
        try
        {
            var model = new DefaultModel();
            DependencySystem.Reset();

            var memory = new DefaultMemoryBehaviour();
            Memory.Reset(memory);

            var implementer = new DefaultImplementer();
            if (debug)
                implementer.Implement();
            
            var loopApp = new LoopNodeAppElement<T>(model);
            
            var chain = new ReloadElement(
                model,
                new HotReload(model),
                loopApp
            );

            model.Run(debug ? chain : loopApp);
            
            model.OnError += (el, er) =>
            {
                Verbose.Error($"On {el} AsyncElement:");
                ShowError(er);
            };
            model.Start();
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }
}