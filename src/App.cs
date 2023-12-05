using System;

namespace Blindness;

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
            throw;
        }
    }
}