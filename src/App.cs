/* Author:  Leonardo Trevisan Silio
 * Date:    01/01/2024
 */
using System;

namespace Blindness;

using States;
using Internal;
using Abstracts;
using Concurrency;
using Concurrency.Elements;

/// <summary>
/// Base class for start a application
/// </summary>
public static class App
{
    public static bool Debug { get; set; } = true;

    public static void StartWith<T>(
        IAsyncModel model = null,
        IMemoryBehaviour memory = null
    ) where T : INode
    {
        try
        {
            model ??= new DefaultModel();
            DependencySystem.Reset(model);

            memory ??= new DefaultMemory();
            Memory.Reset(memory);
            
            if (Debug)
            {
                var implementer = new Implementer();
                implementer.Implement();
            }
            
            var loopApp = new LoopNodeAppElement<T> {
                Model = model
            };

            if (Debug)
            {
                var chain = new ReloadLoopElement {
                    Model = model,
                    First = new HotReload(),
                    Second = loopApp
                };
                model.Run(chain);
            }
            else
            {
                model.Run(loopApp);
            }
            
            model.OnError += (el, er) =>
            {
                Verbose.Error($"On {el} AsyncElement:");
                showError(er);
            };
            model.Start();
        }
        catch (Exception ex)
        {
            showError(ex);
        }
    }

    private static void showError(Exception ex)
    {
        Verbose.Error(ex.Message, -1);

        var lines = ex.StackTrace.Split('\n');
        foreach (var line in lines)
        {
            var isInternal = line
                .Trim()
                .StartsWith("at Blindness");
            
            Verbose.Error(line, isInternal ? 1 : 0);
        }
    }
}