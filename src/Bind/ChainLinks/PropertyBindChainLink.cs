/* Author:  Leonardo Trevisan Silio
 * Date:    08/08/2024
 */
using System;
using System.Reflection;
using System.Linq.Expressions;

namespace Blindness.Bind.ChainLinks;

/// <summary>
/// Represents a chainlink that handle a obj.BindProperty expression.
/// </summary>
public class PropertyBindChainLink : BindChainLink
{
    protected override BindingResult TryHandle(BindingArgs args)
    {
        ArgumentNullException.ThrowIfNull(args, nameof(args));
        ArgumentNullException.ThrowIfNull(args.Body, nameof(args.Body));
        
        var body = args.Body.RemoveTypeCast();
        if (body is not MemberExpression mexp)
            return BindingResult.Unsuccesfull;
        
        var member = mexp.Member;
        if (member.GetCustomAttribute<BindingAttribute>() is null)
            return BindingResult.Unsuccesfull;

        var instanciator = Expression.Lambda(mexp.Expression);
        var obj = instanciator.Compile().DynamicInvoke();
        var binding = Binding.Get(obj);
        var box = binding.Dictionary.GetBox(
            member.Name, member.GetMemberReturnType()
        );

        return BindingResult.Successful(box, binding);
    }
}