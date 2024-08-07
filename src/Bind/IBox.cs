/* Author:  Leonardo Trevisan Silio
 * Date:    30/07/2024
 */
namespace Blindness.Bind;

/// <summary>
/// Represents a variant generic type to set and get a type with boxing.
/// </summary>
public interface IBox<out T, in R>
{
    /// <summary>
    /// Gets if the box is readonly and will throw a error on Place operation.
    /// </summary>
    bool IsReadonly { get; }

    /// <summary>
    /// Open and read the box value.
    /// </summary>
    T Open();

    /// <summary>
    /// Place a new value in the box.
    /// </summary>
    void Place(R value);
}

/// <summary>
/// Represents a variant generic type to set and get a type with boxing.
/// </summary>
public interface IBox<T> : IBox<T, T>;