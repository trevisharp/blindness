/* Author:  Leonardo Trevisan Silio
 * Date:    12/08/2024
 */
namespace Blindness.Reload;

public abstract class Reloader
{
    public FileWatcher Watcher { get; set; } = new();
    public AssemblyCompiler Compiler { get; set; }


}