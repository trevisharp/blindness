/* Author:  Leonardo Trevisan Silio
 * Date:    08/08/2024
 */
using System;
using System.Linq.Expressions;

namespace Blindness.Bind.ChainLinks;

using System.Linq;
using Boxes;

/// <summary>
/// Represents a bind chain link for a.func() expressions.
/// </summary>
public class CallBindChainLink : BindChainLink
{
    protected override BindingResult TryHandle(BindingArgs args)
    {
        ArgumentNullException.ThrowIfNull(args, nameof(args));
        ArgumentNullException.ThrowIfNull(args.Body, nameof(args.Body));

        var body = args.Body.RemoveTypeCast();
        if (body is not MethodCallExpression call)
            return BindingResult.Unsuccesfull;
        
        var instanciator = Expression.Lambda(call.Object).Compile();
        var member = call.Method;
        var arguments =
            from arg in call.Arguments
            select args.Chain.Handle(new(arg, args.Chain));
        if (arguments.Any(a => !a.Success))
            return BindingResult.Unsuccesfull;

        var box = Box.CreateExpression(
            member.GetMemberReturnType(),
            member, instanciator, [ ..arguments.Select(a => a.MainBox) ]
        );

        return BindingResult.Successful(box);
    }
}