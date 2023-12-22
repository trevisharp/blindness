using System;
using System.Text;
using System.Linq;
using System.Diagnostics;

namespace Blindness;

using Internal;
using Elements;
using Concurrency;

public static class App
{
    public static void StartWith<T>(IAsyncModel model = null)
        where T : INode
    {
        try
        {
            model ??= new MainModel();
            var app = DependencySystem
                .Current.GetConcrete(typeof(T));

            var realTime = new RealTimeElement
            {
                Element = app,
                Model = model
            };
            
            model.Run(realTime);
            model.Start();
        }
        catch (Exception ex)
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
}