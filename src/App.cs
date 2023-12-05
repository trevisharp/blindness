using System;
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
            StackTrace st = new StackTrace(ex);
            var lines = ex.StackTrace.Split('\n');
            
            for (int i = 0; i < st.FrameCount; i++)
            {
                var frame = st.GetFrame(i);

                System.Console.WriteLine(frame.HasSource());
                System.Console.WriteLine(frame.GetFileName());
                System.Console.WriteLine(frame.GetFileLineNumber());
                System.Console.WriteLine(frame.GetILOffset());
                System.Console.WriteLine(frame.GetMethod().DeclaringType.Assembly);
                System.Console.WriteLine(lines[i]);
                System.Console.WriteLine();
            }

            System.Console.WriteLine(lines.Length);

            Verbose.Error(ex.Message, -1);
        }
    }
}