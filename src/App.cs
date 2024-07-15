/* Author:  Leonardo Trevisan Silio
 * Date:    15/07/2024
 */
namespace Blindness;

/// <summary>
/// Base class for start a application.
/// </summary>
public static class App
{
    public static bool Debug { get; set; } = true;
    public static AppBehaviour Behaviour { get; set; } = new DefaultAppBehaviour();

    /// <summary>
    /// Start application based on a Node type.
    /// </summary>
    public static void StartWith<T>() where T : INode
        => Behaviour.Run<T>(Debug);
}