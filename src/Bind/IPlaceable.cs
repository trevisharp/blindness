/* Author:  Leonardo Trevisan Silio
 * Date:    31/07/2024
 */
namespace Blindness.Bind;

/// <summary>
/// Represents a contravariant type to save values.
/// </summary>
public interface IPlaceable<in R>
{
    /// <summary>
    /// Place a new internal value to this object.
    /// </summary>
    void Place(R newValue);
}