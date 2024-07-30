/* Author:  Leonardo Trevisan Silio
 * Date:    01/01/2024
 */
using System;
using System.Linq.Expressions;

namespace Blindness;

using Bind;

/// <summary>
/// A base class for all nodes.
/// </summary>
[Ignore]
public interface INode
{
    /// <summary>
    /// Binding property.
    /// </summary>
    Binding<string> Bind { get; set; }

    /// <summary>
    /// Run the node.
    /// </summary>
    void Run();

    /// <summary>
    /// Add a action called every time that condition is true.
    /// </summary>
    void When(
        Func<bool> condition,
        Action action
    );

    /// <summary>
    /// Add a action called every time that condition change.
    /// </summary>
    void On(
        Expression<Func<bool>> condition,
        Action<bool> action
    );
}