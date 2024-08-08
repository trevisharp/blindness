/* Author:  Leonardo Trevisan Silio
 * Date:    08/08/2024
 */
namespace Blindness.Bind.Boxes;

/// <summary>
/// Represents a Box that can store another box.
/// </summary>
public class InnerBox<T>(IBox<T> initial) : IBox<T>
{
    public IBox<T> Inner { get; set; } = initial;

    public bool IsReadonly => Inner?.IsReadonly ?? true;

    public T Open()
    {
        if (Inner is null)
            return default;
        
        return Inner.Open();
    }

    public void Place(T value)
    {
        if (Inner is null)
            return;
        
        Inner.Place(value);
    }
}