/* Author:  Leonardo Trevisan Silio
 * Date:    31/07/2024
 */
namespace Blindness.Bind;

/// <summary>
/// Exposes a function to build a Bind Chain.
/// </summary>
public interface IBindBehaviour
{
    /// <summary>
    /// Build a chain to define the bind operation behaviour.
    /// </summary>
    BindChain BuildChain();
}