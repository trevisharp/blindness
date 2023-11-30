/* Author:  Leonardo Trevisan Silio
 * Date:    30/11/2023
 */
namespace Blindness;

public class Root : Node
{
    public static T New<T>() where T : Root
        => DependencySystem.Current.GetConcrete(typeof(T)) as T;

    public void Run()
    {
        while (true)
            this.Process();
    }
}