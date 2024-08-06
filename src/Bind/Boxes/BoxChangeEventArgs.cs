/* Author:  Leonardo Trevisan Silio
 * Date:    30/07/2024
 */
namespace Blindness.Bind.Boxes;

/// <summary>
/// Represents the arguments of a change value event.
/// </summary>
public record BoxChangeEventArgs<T>(T OldValue, T NewValue);