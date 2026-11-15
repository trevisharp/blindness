/* Author:  Leonardo Trevisan Silio
 * Date:    08/08/2024
 */
using System;

namespace Blindness.Bind.Boxes;

/// <summary>
/// Represents a Box that can store another box.
/// </summary>
public class InnerBox<T> : IBox<T>, INotifyBox
{
    IBox<T> inner;

    public InnerBox(IBox<T> initial)
    {
        inner = initial;
        Subscribe(inner);
    }

    public event Action Changed;

    public IBox<T> Inner
    {
        get => inner;
        set
        {
            if (ReferenceEquals(inner, value))
                return;

            Unsubscribe(inner);
            inner = value;
            Subscribe(inner);
            Changed?.Invoke();
        }
    }

    public bool IsReadonly => Inner?.IsReadonly ?? true;

    public T Open()
    {
        BoxTracker.Read(this);
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

    void OnInnerChanged()
        => Changed?.Invoke();

    void Subscribe(IBox<T> box)
    {
        if (box is INotifyBox notify)
            notify.Changed += OnInnerChanged;
    }

    void Unsubscribe(IBox<T> box)
    {
        if (box is INotifyBox notify)
            notify.Changed -= OnInnerChanged;
    }
}