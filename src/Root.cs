using System;

namespace Blindness;

public class Root : Node
{
    public static T New<T>()
        where T : Root
    {
        try
        {
            return DependencySystem.Current.GetConcrete(typeof(T)) as T;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public void Run()
    {
        while (true)
            this.Process();
    }
}