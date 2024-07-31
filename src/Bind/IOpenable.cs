/* Author:  Leonardo Trevisan Silio
 * Date:    30/07/2024
 */
namespace Blindness.Bind;

/// <summary>
/// Represents a covariant boxing type that can be opened.
/// </summary>
public interface IOpenable<out T>
{
    /// <summary>
    /// Open and get the internal value from this object.
    /// </summary>
    T Open();
}