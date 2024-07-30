/* Author:  Leonardo Trevisan Silio
 * Date:    30/07/2024
 */
using System;

namespace Blindness.Bind;

/// <summary>
/// Represents a structure to box values.
/// </summary>
public class Box<T> : IBox<T, T>
{
    T value;
    public Box(T initialValue = default)
        => value = initialValue;
    public Box(Box<T> other)
        => value = other.Open();

    public event Action<BoxChangeEventArgs<T>> OnChange;
    public T Open()
        => value;
    public void Place(T newValue)
    {
        if (newValue.Equals(value))
            return;
        
        if (OnChange is not null)
            OnChange(new(value, newValue));
        value = newValue;
    }

    public static implicit operator T(Box<T> pointer)
        => pointer.value;
}