/* Author:  Leonardo Trevisan Silio
 * Date:    12/08/2024
 */
using System;
using System.Reflection;
using System.Linq.Expressions;

namespace Blindness.Core.Binds;

using Bind;

/// <summary>
/// Represents a chainlink that handle a obj.BindProperty with a Deep Binding delcaration.
/// </summary>
public class DeepPropertyBindChainLink : BindChainLink
{
    protected override BindingResult TryHandle(BindingArgs args)
    {
        ArgumentNullException.ThrowIfNull(args, nameof(args));
        ArgumentNullException.ThrowIfNull(args.Body, nameof(args.Body));
        
        var body = args.Body.RemoveTypeCast();
        if (body is not MemberExpression mexp)
            return BindingResult.Unsuccesfull;
        
        var member = mexp.Member;
        var instanciator = Expression.Lambda(mexp.Expression);
        var obj = instanciator.Compile().DynamicInvoke();

        var deepMember = obj.GetType().GetProperty(member.Name);
        if (deepMember.GetCustomAttribute<BindingAttribute>() is null)
            return BindingResult.Unsuccesfull;
        
        var binding = Binding.Get(obj);
        var box = binding.Dictionary.GetBox(
            member.Name, member.GetMemberReturnType()
        );

        return BindingResult.Successful(box, binding);
    }
}