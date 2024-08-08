/* Author:  Leonardo Trevisan Silio
 * Date:    07/08/2024
 */
namespace Blindness.Bind.Analyzers;

using ChainLinks;

/// <summary>
/// The default implementation of IBindAnalyzer for left side of expression.
/// </summary>
public class DefaultLeftBindAnalyzer : IBindAnalyzer
{
    public BindChain BuildChain() => 
        BindChain.New()
        .Add(new ExpressionBindChainLink());
}