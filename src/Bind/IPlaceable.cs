/* Author:  Leonardo Trevisan Silio
 * Date:    30/07/2024
 */
namespace Blindness.Bind;

/// <summary>
/// Represents a contravariant type to save values.
/// </summary>
public interface IPlaceable<in R>
{
    void Place(R newValue);
}