/* Author:  Leonardo Trevisan Silio
 * Date:    14/08/2024
 */
namespace Blindness.Bind.Analyzers;

using ChainLinks;

/// <summary>
/// The default implementation of IBindAnalyzer for right side of expression.
/// </summary>
public class DefaultRightBindAnalyzer : IBindAnalyzer
{
    public BindChain BuildChain() => 
        BindChain.New()
        .Add(new BinaryOperationBindChainLink())
        .Add(new ConditionalBindChainLink())
        .Add(new ExpressionBindChainLink())
        .Add(new CallBindChainLink())
        .Add(new ConstantBindChainLink());
}