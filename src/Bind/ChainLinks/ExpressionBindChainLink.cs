/* Author:  Leonardo Trevisan Silio
 * Date:    07/08/2024
 */
using System;
using System.Linq.Expressions;

namespace Blindness.Bind.ChainLinks;

using Boxes;

/// <summary>
/// Bind a expression of type A => B.C ... .
/// </summary>
public class ExpressionBindChainLink : BindChainLink
{
    protected override BindingResult TryHandle(BindingArgs args)
    {
        ArgumentNullException.ThrowIfNull(args, nameof(args));
        ArgumentNullException.ThrowIfNull(args.Body, nameof(args.Body));

        var body = args.Body.RemoveTypeCast();
        if (body is not MemberExpression mexp)
            return BindingResult.Unsuccesfull;
        
        var instanciator = Expression.Lambda(mexp.Expression);
        var member = mexp.Member;

        var box = Box.CreateExpression(
            member.GetMemberReturnType(),
            member, instanciator
        );

        return BindingResult.Successful(box);
    }
}