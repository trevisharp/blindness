/* Author:  Leonardo Trevisan Silio
 * Date:    16/11/2026
 */
namespace Blindness.Bind;

/// <summary>
/// Represents a object that holds its own Binding.
/// </summary>
public interface IBindable
{
    /// <summary>
    /// Get the binding of this object.
    /// </summary>
    Binding Binding { get; }
}
