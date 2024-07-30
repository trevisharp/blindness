/* Author:  Leonardo Trevisan Silio
 * Date:    30/07/2024
 */
namespace Blindness.Bind;

/// <summary>
/// Represents a variant generic type to set and get a type with boxing.
/// </summary>
public interface IBox<out T, in R> : IOpenable<T>, IPlaceable<R>;