/* Author:  Leonardo Trevisan Silio
 * Date:    01/01/2024
 */
namespace Blindness;

/// <summary>
/// Base class for start a application
/// </summary>
public static class App
{
    public static bool Debug { get; set; } = true;
    public static AppBehaviour Behaviour { get; set; } = new DefaultAppBehaviour();

    public static void StartWith<T>() where T : INode
        => Behaviour.Run<T>(Debug);
}