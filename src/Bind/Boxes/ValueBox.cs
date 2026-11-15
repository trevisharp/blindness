/* Author:  Leonardo Trevisan Silio
 * Date:    05/08/2024
 */
using System;
using System.Collections.Generic;

namespace Blindness.Bind.Boxes;

/// <summary>
/// Represents a structure to box values.
/// </summary>
public class ValueBox<T> : IBox<T>, INotifyBox
{
    T value;

    public ValueBox(T initialValue = default)
        => value = initialValue;

    public ValueBox(ValueBox<T> other)
        => value = other.Open();

    public bool IsReadonly => false;

    /// <summary>
    /// This events is triggered every moment that a new value
    /// is placed and is diferent of the old internal value.
    /// </summary>
    public event Action<BoxChangeEventArgs<T>> OnChange;

    public event Action Changed;

    public T Open()
    {
        BoxTracker.Read(this);
        return value;
    }

    public void Place(T newValue)
    {
        if (EqualityComparer<T>.Default.Equals(newValue, value))
            return;

        var oldValue = value;
        value = newValue;
        OnChange?.Invoke(new(oldValue, newValue));
        Changed?.Invoke();
    }

    public static implicit operator T(ValueBox<T> pointer)
        => pointer.value;
}
