/* Author:  Leonardo Trevisan Silio
 * Date:    14/08/2024
 */
using System;

namespace Blindness.Bind.Boxes;

using Exceptions;

/// <summary>
/// Represents a readonly constant box.
/// </summary>
public class ConditionalBox<T>(Func<bool> condition, Func<T> ifTrue, Func<T> ifFalse) : IBox<T>
{
    public bool IsReadonly => true;

    public T Open()
        => condition() ? ifTrue() : ifFalse();

    public void Place(T value)
        => throw new ReadonlyBoxException();
}