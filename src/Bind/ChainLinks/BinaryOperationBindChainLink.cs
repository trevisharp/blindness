/* Author:  Leonardo Trevisan Silio
 * Date:    07/08/2024
 */
namespace Blindness.Bind.ChainLinks;

using System.Linq.Expressions;
using Boxes;

/// <summary>
/// Represents a bind chain link for a + b expressions.
/// </summary>
public class BinaryOperationBindChainLink : BindChainLink
{
    protected override bool TryHandle(BindingArgs args, out BindingResult result)
    {
        result = new();
        var body = args.Body.RemoveTypeCast();
        if (body is not BinaryExpression bin)
            return false;
        
        var subArgs1 = args with { Body = bin.Left };
        if (!args.Chain.Handle(subArgs1, out var res1))
            return false;

        var subArgs2 = args with { Body = bin.Right };
        if (!args.Chain.Handle(subArgs2, out var res2))
            return false;
        
        

        return true;
    }
}