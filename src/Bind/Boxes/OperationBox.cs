/* Author:  Leonardo Trevisan Silio
 * Date:    07/08/2024
 */
using System;

namespace Blindness.Bind.Boxes;

using Exceptions;

/// <summary>
/// Represents a box that has a operation of two value has internal value.
/// </summary>
public class OperationBox<T>(
    IBox<T, T> leftBox,
    IBox<T, T> rightBox,
    Func<T, T, T> operation,
    Func<T, T, T> computeLeft,
    Func<T, T, T> computeRight) : IBox<T>
{
    public bool IsReadonly => 
        (leftBox.IsReadonly || computeRight is null) && 
        (rightBox.IsReadonly || computeLeft is null);

    public T Open()
        => (T)operation.DynamicInvoke([ leftBox.Open(), rightBox.Open() ]);

    public void Place(T newValue)
    {
        if (!leftBox.IsReadonly && computeLeft is not null)
        {
            var reversedValue = computeLeft.DynamicInvoke([ newValue, rightBox.Open() ]);
            leftBox.Place((T)reversedValue);
            return;
        }

        if (!rightBox.IsReadonly && computeRight is not null)
        {
            var reversedValue = computeRight.DynamicInvoke([ newValue, leftBox.Open() ]);
            rightBox.Place((T)reversedValue);
            return;
        }

        throw new ReadonlyBoxException();
    }
}