using System;

namespace Blindness;

using States;
using Internal;
using Abstracts;
using Concurrency;
using Concurrency.Elements;

public static class App
{
    public static void StartWith<T>(
        IAsyncModel model = null,
        IMemoryBehaviour memory = null
    ) where T : INode
    {
        try
        {
            // var implementer = new Implementer();
            // implementer.Implement();

            HotReload.IsActive = true;

            model ??= new DefaultModel();
            DependencySystem.Reset(model);

            memory ??= new DefaultMemory();
            Memory.Reset(memory);

            var app = DependencySystem
                .Current.GetConcrete(typeof(T));
            
            var realTime = new RealTimeElement<T>
            {
                ElementPointer = Memory.Current.Add(app),
                Model = model
            };
            
            model.Run(realTime);
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