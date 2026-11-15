/* Author:  Leonardo Trevisan Silio
 * Date:    14/11/2026
 */
using System;

namespace Blindness.Bind.Boxes;

using Exceptions;

/// <summary>
/// Represents a box that applies a unary operation over the value of another box.
/// </summary>
public class UnaryBox<T, R>(
    IBox<T, T> inner,
    Func<T, R> forward,
    Func<R, T> backward) : IBox<R>, INotifyBox
{
    public event Action Changed
    {
        add
        {
            if (inner is INotifyBox notify)
                notify.Changed += value;
        }
        remove
        {
            if (inner is INotifyBox notify)
                notify.Changed -= value;
        }
    }

    public bool IsReadonly => backward is null || inner.IsReadonly;

    public R Open()
        => forward(inner.Open());

    public void Place(R value)
    {
        if (IsReadonly)
            throw new ReadonlyBoxException();

        inner.Place(backward(value));
    }
}
