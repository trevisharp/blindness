/* Author:  Leonardo Trevisan Silio
 * Date:    16/11/2026
 */
using System;
using System.Threading;

namespace Blindness;

/// <summary>
/// Represents a registration that is undone once when disposed.
/// </summary>
public sealed class Subscription : IDisposable
{
    Action undo;

    public Subscription(Action undo)
    {
        ArgumentNullException.ThrowIfNull(undo, nameof(undo));
        this.undo = undo;
    }

    public void Dispose()
        => Interlocked.Exchange(ref undo, null)?.Invoke();
}
