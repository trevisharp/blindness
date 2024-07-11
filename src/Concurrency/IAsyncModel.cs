/* Author:  Leonardo Trevisan Silio
 * Date:    11/07/2024
 */
using System;

namespace Blindness.Concurrency;

/// <summary>
/// Represents a async model used to manage concurrency system.
/// </summary>
public interface IAsyncModel
{
    /// <summary>
    /// Start to running the model.
    /// Autostop when the model has no elementos to Run.
    /// </summary>
    void Start();

    /// <summary>
    /// Run a element on AsyncModel.
    /// </summary>
    void Run(IAsyncElement node);

    /// <summary>
    /// Stop the AsyncModel.
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