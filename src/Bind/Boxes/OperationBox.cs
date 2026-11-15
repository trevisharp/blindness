/* Author:  Leonardo Trevisan Silio
 * Date:    14/08/2024
 */
using System;

namespace Blindness.Bind.Boxes;

using Exceptions;

/// <summary>
/// Represents a box that has a operation of two value has internal value.
/// </summary>
public class OperationBox<T1, T2, R>(
    IBox<T1, T1> leftBox,
    IBox<T2, T2> rightBox,
    Func<T1, T2, R> operation,
    Func<T1, T2, R> computeLeft,
    Func<T1, T2, R> computeRight) : IBox<R>, INotifyBox
{
    public event Action Changed
    {
        add
        {
            if (leftBox is INotifyBox left)
                left.Changed += value;
            if (rightBox is INotifyBox right)
                right.Changed += value;
        }
        remove
        {
            if (leftBox is INotifyBox left)
                left.Changed -= value;
            if (rightBox is INotifyBox right)
                right.Changed -= value;
        }
    }

    public bool IsReadonly =>
        (leftBox.IsReadonly || computeLeft is null) &&
        (rightBox.IsReadonly || computeRight is null);

    public R Open()
        => operation(leftBox.Open(), rightBox.Open());

    public void Place(R newValue)
    {
        if (!leftBox.IsReadonly && computeLeft is not null)
        {
            var reversedValue = computeLeft((T1)(object)newValue, rightBox.Open());
            leftBox.Place((T1)(object)reversedValue);
            return;
        }

        if (!rightBox.IsReadonly && computeRight is not null)
        {
            var reversedValue = computeRight((T1)(object)newValue, (T2)(object)leftBox.Open());
            rightBox.Place((T2)(object)reversedValue);
            return;
        }

        throw new ReadonlyBoxException();
    }
}
