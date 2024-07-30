/* Author:  Leonardo Trevisan Silio
 * Date:    30/07/2024
 */
namespace Blindness.Bind;

/// <summary>
/// Represents a structure to box values.
/// </summary>
public class Box<T>(T initalValue = default)
{
    T value = initalValue;
    public T Value => value;

    public void SetValue(T newValue)
    {
        if (newValue.Equals(value))
            return;
        
        value = newValue;
    }

    public static implicit operator T(Box<T> pointer)
        => pointer.value;
}