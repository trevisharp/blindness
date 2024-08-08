/* Author:  Leonardo Trevisan Silio
 * Date:    07/08/2024
 */
namespace Blindness.Bind;

/// <summary>
/// Exposes a function to build a Bind Chain.
/// </summary>
public interface IBindAnalyzer
{
    /// <summary>
    /// Build a chain to define the bind operation behaviour.
    /// </summary>
    BindChain BuildChain();
}