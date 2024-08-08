/* Author:  Leonardo Trevisan Silio
 * Date:    07/08/2024
 */
namespace Blindness.Bind.ChainLinks;

using Boxes;
using System.Linq.Expressions;

/// <summary>
/// Represents a bind chain link for a + b expressions.
/// </summary>
public class BinaryOperationBindChainLink : BindChainLink
{
    protected override BindingResult TryHandle(BindingArgs args)
    {
        var body = args.Body.RemoveTypeCast();
        if (body is not BinaryExpression bin)
            return BindingResult.Unsuccesfull;
        
        var subArgs1 = args with { Body = bin.Left };
        var res1 = args.Chain.Handle(subArgs1);
        if (!res1.Success)
            return BindingResult.Unsuccesfull;

        var subArgs2 = args with { Body = bin.Right };
        var res2 = args.Chain.Handle(subArgs2);
        if (!res2.Success)
            return BindingResult.Unsuccesfull;
        
        
    }
}