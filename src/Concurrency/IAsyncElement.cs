/* Author:  Leonardo Trevisan Silio
 * Date:    11/07/2024
 */
using System;

namespace Blindness.Concurrency;

/// <summary>
/// Represents a element that can run in a AsyncModel
/// </summary>
public interface IAsyncElement
{
    /// <summary>
    /// Get the current element model.
    /// </summary>
    IAsyncModel Model { get; }

    /// <summary>
    /// A event launched on every characteristic operation step
    /// executed by the AsyncElement.
    /// </summary>
    event Action<IAsyncElement> OnSignal;

    /// <summary>
    /// Run code from the element.
    /// </summary>
    void Start();

    /// <summary>
    /// Stop current thread to wait the element complete a operation step.
    /// </summary>
    void Wait();

    /// <summary>
    /// Stop the element.
    /// </summary>
    void Finish();
}