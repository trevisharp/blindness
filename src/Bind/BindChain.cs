/* Author:  Leonardo Trevisan Silio
 * Date:    06/08/2024
 */
using System;

namespace Blindness.Bind;

/// <summary>
/// Represents a chain of elements that can handle a binding request.
/// </summary>
public class BindChain
{
    BindChainLink first = null;
    BindChainLink last = null;

    /// <summary>
    /// Add a new BindChainLink to chain.
    /// </summary>
    public BindChain Add(BindChainLink link)
    {
        ArgumentNullException.ThrowIfNull(link);
        first ??= link;

        if (last is not null)
            last.Next = link;
        last = link;
        
        return this;
    }

    /// <summary>
    /// Handle the binding args in the nodes of the chain.
    /// </summary>
    public bool Handle(BindingArgs args)
        => first?.Handle(args) ?? false;

    /// <summary>
    /// Handle the binding args in the nodes of the chain.
    /// </summary>
    public bool Handle(BindingArgs args, out BindingResult result)
    {
        result = new();
        return first?.Handle(args, out result) ?? false;
    }
    
    /// <summary>
    /// Create a new empty BindChain.
    /// </summary>
    public static BindChain New()
        => new();
}