/* Author:  Leonardo Trevisan Silio
 * Date:    07/08/2024
 */
namespace Blindness.Bind;

/// <summary>
/// Exposes a function get bind result from a bind analisys and make bind operations.
/// </summary>
public interface IBindBehavior
{
    /// <summary>
    /// Build a chain to define the bind operation behaviour.
    /// </summary>
    void MakeBinding(BindingResult left, BindingResult right);
}