/* Author:  Leonardo Trevisan Silio
 * Date:    06/08/2024
 */
using System;
using System.Reflection;

namespace Blindness.Bind.Boxes;

using Exceptions;

/// <summary>
/// Represents a box that has a operation of two value has internal value.
/// </summary>
public class OperationBox<T>(
    IBox<T, T> leftBox,
    IBox<T, T> rightBox,
    MethodInfo operation,
    Func<T, T, T> reverseLeft = null,
    Func<T, T, T> reverseRight = null) : IBox<T>
{   
    public T Open()
        => (T)operation.Invoke(null, [ leftBox.Open(), rightBox.Open() ]);

    public void Place(T newValue)
    {
        if (leftBox is ConstantBox<T> && reverseLeft is not null)
        {
            var reversedValue = reverseLeft(leftBox.Open(), newValue);
            rightBox.Place(reversedValue);
            return;
        }

        if (rightBox is ConstantBox<T> && reverseRight is not null)
        {
            var reversedValue = reverseRight(newValue, rightBox.Open());
            leftBox.Place(reversedValue);
            return;
        }

        throw new ReadonlyBoxException();
    }
}