/* Author:  Leonardo Trevisan Silio
 * Date:    30/07/2024
 */
using System;

namespace Blindness.Bind;

using Exceptions;

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

    /// <summary>
    /// This events is triggered every moment that a new value
    /// is placed and is diferent of the old internal value.
    /// </summary>
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

public static class Box
{
    /// <summary>
    /// Create a Box from a type.
    /// </summary>
    public static object Create(Type type)
    {
        var boxType = typeof(Box<>);
        var genBoxType = boxType.MakeGenericType(type);
        var boxConstructor = genBoxType.GetConstructor([ type ]);
        var boxObj = boxConstructor.Invoke([]);
        return boxObj;
    }
    
    /// <summary>
    /// Test if a object is a Box<T>
    /// </summary>
    public static bool IsBox(object value)
        => value.GetType().GetGenericTypeDefinition() == typeof(Box<>);

    /// <summary>
    /// Try to open a object like a box. Throw a exception if
    /// the object is not a box.
    /// </summary>
    public static object Open(object box)
    {
        ArgumentNullException.ThrowIfNull(box, nameof(box));
        BoxTypeException.ThrowIfIsNotABox(box);

        var boxType = box.GetType();
        var placeMethod = boxType.GetMethod("Open");
        return placeMethod.Invoke(box, []);
    }
    
    /// <summary>
    /// Try to place a value in a object like a box. Throw a exception if
    /// the object is not a box.
    /// </summary>
    public static void Place(object box, object value)
    {
        ArgumentNullException.ThrowIfNull(box, nameof(box));
        BoxTypeException.ThrowIfIsNotABox(box);
        
        var boxType = box.GetType();
        var placeMethod = boxType.GetMethod("Place");
        placeMethod.Invoke(box, [ value ]);
    }
}