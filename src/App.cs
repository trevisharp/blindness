using System;
using System.Text;
using System.Diagnostics;

namespace Blindness;

using System.Linq;
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
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(ex.Message);

            var lines = ex.StackTrace.Split('\n');
            foreach (var line in lines)
            {
                var isInternal = line
                    .Trim()
                    .StartsWith("at Blindness");
                
                if (isInternal)
                    continue;
                
                sb.AppendLine(line);
            }

            Verbose.Error(sb.ToString(), -1);
        }
    }
}