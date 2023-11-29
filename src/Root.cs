using System;

namespace Blindness;

public class Root : Node<Root>
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
}