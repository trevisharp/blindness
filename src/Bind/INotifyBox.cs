/* Author:  Leonardo Trevisan Silio
 * Date:    16/11/2026
 */
using System;

namespace Blindness.Bind;

/// <summary>
/// Represents a box that notifies when its value may have changed.
/// </summary>
public interface INotifyBox
{
    /// <summary>
    /// Occurs when the value of the box may have changed.
    /// </summary>
    event Action Changed;
}
