/* Author:  Leonardo Trevisan Silio
 * Date:    06/08/2024
 */
using System;
using System.Reflection;
using Blindness.Exceptions;

namespace Blindness.Bind.Boxes;

/// <summary>
/// Represents a structure to box values.
/// </summary>
public class OperationBox<T>(
    IBox<T, T> boxA,
    IBox<T, T> boxB,
    MethodInfo operation,
    MethodInfo reverseOp = null) : IBox<T>
{   
    public T Open()
        => (T)operation.Invoke(null, [ boxA.Open(), boxB.Open() ]);

    public void Place(T newValue)
    {
        if (reverseOp is null)
            throw new ReadonlyBoxException();

        if (boxA is ConstantBox<T> cb)
        {
            reverseOp()
        }
    }
}