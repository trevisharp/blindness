/* Author:  Leonardo Trevisan Silio
 * Date:    07/08/2024
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
    Func<T1, T2, R> computeRight) : IBox<R>
{
    public bool IsReadonly => 
        (leftBox.IsReadonly || computeRight is null) && 
        (rightBox.IsReadonly || computeLeft is null);

    public R Open()
        => (R)operation.DynamicInvoke([ leftBox.Open(), rightBox.Open() ]);

    public void Place(R newValue)
    {
        if (!leftBox.IsReadonly && computeLeft is not null)
        {
            var reversedValue = computeLeft.DynamicInvoke([ newValue, rightBox.Open() ]);
            leftBox.Place((T1)reversedValue);
            return;
        }

        if (!rightBox.IsReadonly && computeRight is not null)
        {
            var reversedValue = computeRight.DynamicInvoke([ newValue, leftBox.Open() ]);
            rightBox.Place((T2)reversedValue);
            return;
        }

        throw new ReadonlyBoxException();
    }
}