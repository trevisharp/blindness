/* Author:  Leonardo Trevisan Silio
 * Date:    08/08/2024
 */
namespace Blindness.Core.Binds;

using Bind;
using Bind.ChainLinks;

/// <summary>
/// The default implementation of IBindAnalyzer for left side of expression.
/// </summary>
public class DeepLeftBindAnalyzer : IBindAnalyzer
{
    public BindChain BuildChain() => 
        BindChain.New()
        .Add(new DeepPropertyBindChainLink());
}