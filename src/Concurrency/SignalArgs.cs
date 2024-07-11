/* Author:  Leonardo Trevisan Silio
 * Date:    11/07/2024
 */
namespace Blindness.Concurrency;

/// <summary>
/// Represents a base class for all Signal Events that contains all
/// arguments on event call.
/// </summary>
public record SignalArgs(bool Success)
{
    public readonly static SignalArgs True = new(true);
    public readonly static SignalArgs False = new(false);
}