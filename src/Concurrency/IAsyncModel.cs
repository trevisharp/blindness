/* Author:  Leonardo Trevisan Silio
 * Date:    01/01/2024
 */
using System;

namespace Blindness.Concurrency;

/// <summary>
/// Represents a async model used to manage concurrency system.
/// </summary>
public interface IAsyncModel
{
    /// <summary>
    /// Start running the model.
    /// </summary>
    void Start();

    /// <summary>
    /// Run a element.
    /// </summary>
    void Run(IAsyncElement node);

    /// <summary>
    /// Stop running the model.
    /// </summary>
    void Stop();

    /// <summary>
    /// Event called every error inside every element started from the model.
    /// </summary>
    event Action<IAsyncElement, Exception> OnError;

    /// <summary>
    /// Send a error to the main model.
    /// </summary>
    void SendError(IAsyncElement el, Exception ex);
}