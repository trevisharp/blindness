/* Author:  Leonardo Trevisan Silio
 * Date:    07/08/2024
 */
namespace Blindness.Bind.Behaviors;

using Boxes;
using Exceptions;

/// <summary>
/// The default implementation of IBindBehaivour.
/// </summary>
public class DefaultBindBehavior : IBindBehavior
{
    public void MakeBinding(BindingResult left, BindingResult right)
    {
        var leftReadonly = Box.IsReadOnly(left.MainBox);
        var rightReadonly = Box.IsReadOnly(right.MainBox);
        
        if (leftReadonly && rightReadonly)
            throw new ReadonlyBindingException();
        
        if (!leftReadonly)
        {

        }
    }
}