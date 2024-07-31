/* Author:  Leonardo Trevisan Silio
 * Date:    31/07/2024
 */
namespace Blindness.Bind;

/// <summary>
/// The default implementation of IBindBehaivour.
/// </summary>
public class DefaultBindBehaviour : IBindBehaviour
{
    public BindChain BuildChain() => 
        BindChain.New();
}