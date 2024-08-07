/* Author:  Leonardo Trevisan Silio
 * Date:    06/08/2024
 */
namespace Blindness.Bind.Boxes;

using Exceptions;

/// <summary>
/// Represents a readonly constant box.
/// </summary>
public class ConstantBox<T>(T value) : IBox<T>
{
    public bool IsReadonly => true;

    public T Open()
        => value;

    public void Place(T value)
        => throw new ReadonlyBoxException();
}