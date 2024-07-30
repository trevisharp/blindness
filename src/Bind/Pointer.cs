/* Author:  Leonardo Trevisan Silio
 * Date:    30/07/2024
 */
using System.Collections.Generic;

namespace Blindness.Bind;

public class Pointer<T>(T initalValue = default)
    where T : class
{
    T value = initalValue;
    public T Value => value;

    public void SetValue(T value)
    {

    }

    public static implicit operator T(Pointer<T> pointer)
        => pointer.value;
}