using System;
using System.Text;
using System.Linq;
using System.Diagnostics;

namespace Blindness;

using Internal;

public static class App
{
    public static void RunNode<T>()
        where T : INode
    {
        try
        {
            var app = DependencySystem
                .Current.GetConcrete(typeof(T));
            while (true)
            {
                app.Process();
            }
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