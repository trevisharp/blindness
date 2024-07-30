/* Author:  Leonardo Trevisan Silio
 * Date:    30/07/2024
 */
using System;

namespace Blindness.Bind;

/// <summary>
/// Represents a structure to box values.
/// </summary>
public class Box<T>(T initalValue = default)
{
    T value = initalValue;
    public T Value => value;
    
    public event Action<BoxChangeEventArgs<T>> OnChange;

    public void SetValue(T newValue)
    {
        if (newValue.Equals(value))
            return;
        
        OnChange(new(value, newValue));
        value = newValue;
    }

    public static implicit operator T(Box<T> pointer)
        => pointer.value;
}