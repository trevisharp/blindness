/* Author:  Leonardo Trevisan Silio
 * Date:    07/08/2024
 */
using System;

namespace Blindness.Bind.Behaviors;

using Boxes;
using Exceptions;

/// <summary>
/// The default implementation of IBindBehaivour.
/// </summary>
public class DefaultBindBehavior : IBindBehavior
{
    public IDisposable MakeBinding(BindingResult left, BindingResult right)
    {
        var leftReadonly = Box.IsReadOnly(left.MainBox);
        var rightReadonly = Box.IsReadOnly(right.MainBox);

        if (leftReadonly && rightReadonly)
            throw new ReadonlyBindingException();

        Box.SetInner(left.MainBox, right.MainBox);
        return new Subscription(() => Unbind(left.MainBox, right.MainBox));
    }

    /// <summary>
    /// Detach a left box from the right box keeping its current value.
    /// Nothing happens if the left box was bound to another box meanwhile.
    /// </summary>
    static void Unbind(object leftBox, object rightBox)
    {
        if (!ReferenceEquals(Box.GetInner(leftBox), rightBox))
            return;

        var snapshot = Box.Create(Box.GetBoxType(leftBox));
        Box.Place(snapshot, Box.Open(leftBox));
        Box.SetInner(leftBox, snapshot);
    }
}