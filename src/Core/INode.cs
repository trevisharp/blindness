/* Author:  Leonardo Trevisan Silio
 * Date:    12/08/2024
 */
using System;
using System.Linq.Expressions;

namespace Blindness.Core;

using Factory;

/// <summary>
/// A base class for all nodes.
/// </summary>
[Ignore]
public interface INode
{
    /// <summary>
    /// Load the node data.
    /// </summary>
    void Load();

    /// <summary>
    /// Run the node.
    /// </summary>
    void Run();

    /// <summary>
    /// Unload the node, releasing its events and bindings.
    /// </summary>
    void Unload();

    /// <summary>
    /// Bind properties.
    /// Function to easely call Blindness.Bind.Binding.Bind();
    /// </summary>
    IDisposable Bind(Expression<Func<bool>> binding);

    /// <summary>
    /// Add a action called every time that condition is true.
    /// </summary>
    IDisposable When(
        Func<bool> condition,
        Action action
    );

    /// <summary>
    /// Add a action called every time that condition change.
    /// </summary>
    IDisposable On(
        Func<bool> condition,
        Action<bool> action
    );
}