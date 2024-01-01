/* Author:  Leonardo Trevisan Silio
 * Date:    01/01/2024
 */
namespace Blindness.Concurrency;

/// <summary>
/// Represents a element that can run in a AsyncModel
/// </summary>
public interface IAsyncElement
{
    /// <summary>
    /// Run code from the element.
    /// </summary>
    void Start();

    /// <summary>
    /// Stop current thread to wait the element complete a operation step.
    /// </summary>
    void Await();

    /// <summary>
    /// Stop the element.
    /// </summary>
    void Finish();
}