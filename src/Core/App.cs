/* Author:  Leonardo Trevisan Silio
 * Date:    12/08/2024
 */
using System;

namespace Blindness.Core;

/// <summary>
/// A global object for start and manage your application.
/// </summary>
public static class App
{
    public static bool Debug { get; set; } = true;
    public static AppBehaviour Behaviour { get; set; } = new DefaultAppBehaviour();
    
    public static void Start<T>() where T : INode
    {
        ArgumentNullException.ThrowIfNull(Behaviour, nameof(Behaviour));
        Behaviour.Run<T>();
    }
}