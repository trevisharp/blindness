/* Author:  Leonardo Trevisan Silio
 * Date:    01/01/2024
 */
using System;

namespace Blindness.States;

/// <summary>
/// A inteface for all memory implementations.
/// </summary>
public interface IMemoryBehaviour
{
    /// <summary>
    /// Add an object and returns yours memory address.
    /// </summary>
    int Add(object obj);

    /// <summary>
    /// Get an object and returns yours value at a memory address.
    /// </summary>
    object Get(int address);

    /// <summary>
    /// Find the pointer of a object.
    /// </summary>
    int Find(object obj);

    /// <summary>
    /// Set an object at a memory address.
    /// </summary>
    void Set(int address, object value);

    /// <summary>
    /// Reload all memory values based on a transform function.
    /// </summary>
    void Reload(Func<object, object> func);
}