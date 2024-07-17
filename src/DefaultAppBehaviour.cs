/* Author:  Leonardo Trevisan Silio
 * Date:    15/07/2024
 */
using System;

namespace Blindness;

using States;
using Abstracts;
using Concurrency;
using Concurrency.Elements;

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
            DependencySystem.Reset(model);

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