/* Author:  Leonardo Trevisan Silio
 * Date:    16/11/2026
 */
using System;
using System.Collections.Generic;

namespace Blindness.Bind;

/// <summary>
/// Tracks the boxes read while a function runs.
/// </summary>
public static class BoxTracker
{
    [ThreadStatic]
    static HashSet<INotifyBox> current;

    /// <summary>
    /// Register a box read on the current tracking, if any.
    /// </summary>
    public static void Read(INotifyBox box)
        => current?.Add(box);

    /// <summary>
    /// Run a function registering on reads the boxes opened by it.
    /// </summary>
    public static T Track<T>(Func<T> function, HashSet<INotifyBox> reads)
    {
        ArgumentNullException.ThrowIfNull(function, nameof(function));
        ArgumentNullException.ThrowIfNull(reads, nameof(reads));

        var previous = current;
        current = reads;
        try
        {
            return function();
        }
        finally
        {
            current = previous;
        }
    }
}
