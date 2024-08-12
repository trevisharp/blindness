/* Author:  Leonardo Trevisan Silio
 * Date:    12/08/2024
 */
namespace Blindness.Reload;

/// <summary>
/// The default reloader for C# files verification only.
/// </summary>
public class DefaultReloader : Reloader
{
    public DefaultReloader()
    {
        Watcher = new FileWatcher()
            .AddCSharpFilter();
        
        Compiler = new AssemblyCompiler();
    }
}