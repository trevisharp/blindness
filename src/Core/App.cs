/* Author:  Leonardo Trevisan Silio
 * Date:    12/08/2024
 */
using System;
using System.Reflection;
using System.Diagnostics;

namespace Blindness.Core;

/// <summary>
/// A global object for start and manage your application.
/// </summary>
public static class App
{
    /// <summary>
    /// Enable code generation and hot reload. True by default only on Debug builds.
    /// </summary>
    public static bool Debug { get; set; } = IsDebugBuild();

    static bool IsDebugBuild()
        => Assembly.GetEntryAssembly()?
            .GetCustomAttribute<DebuggableAttribute>()?
            .IsJITOptimizerDisabled ?? false;
    public static AppBehaviour Behaviour { get; set; } = new DefaultAppBehaviour();
    
    public static void Start<T>() where T : INode
    {
        ArgumentNullException.ThrowIfNull(Behaviour, nameof(Behaviour));
        Behaviour.Run<T>();
    }
}