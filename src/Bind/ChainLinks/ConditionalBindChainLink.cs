/* Author:  Leonardo Trevisan Silio
 * Date:    14/08/2024
 */
using System;
using System.Linq.Expressions;

namespace Blindness.Bind.ChainLinks;

using Boxes;

/// <summary>
/// Represents a bind chain link for a.func() expressions.
/// </summary>
public class ConditionalBindChainLink : BindChainLink
{
    protected override BindingResult TryHandle(BindingArgs args)
    {
        ArgumentNullException.ThrowIfNull(args, nameof(args));
        ArgumentNullException.ThrowIfNull(args.Body, nameof(args.Body));

        var body = args.Body.RemoveTypeCast();
        if (body is not ConditionalExpression cond)
            return BindingResult.Unsuccesfull;
        
        var box = Box.CreateConditional(
            cond.IfTrue.Type,
            Expression.Lambda(cond.Test).Compile(),
            Expression.Lambda(cond.IfTrue).Compile(),
            Expression.Lambda(cond.IfFalse).Compile()
        );

        return BindingResult.Successful(box);
    }
}