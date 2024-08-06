/* Author:  Leonardo Trevisan Silio
 * Date:    02/08/2024
 */
using System;
using System.Reflection;
using System.Linq.Expressions;

namespace Blindness.Bind.ChainLinks;

using Exceptions;

/// <summary>
/// Bind a expression of type A => B.C ... .
/// </summary>
public class BaseMemberBindChainLink : BindChainLink
{
    protected override bool TryHandle(BindingArgs args)
    {
        ArgumentNullException.ThrowIfNull(args.Binding, nameof(args.Binding));

        var body = args.Body.RemoveTypeCast();
        if (body is not MemberExpression parent)
            return false;
        
        if (args.Parameters.Count > 1)
            throw new WrongFormatBindException("A => B.C", "(A, B, ...) => W.Z");
        
        if (args.Parameters.Count == 0)
            throw new WrongFormatBindException("A => B.C", "() => B.C");

        var param = args.Parameters[0].Name;
        var (obj, member) = parent.SplitMember();
        var memberType = 
            member is PropertyInfo p ? p.PropertyType :
            member is FieldInfo f ? f.FieldType :
            throw new MissingPropertyBindException(member.Name, obj.GetType());
        var isBindingProp = member.GetCustomAttribute<BindingAttribute>() is not null;

        var bindA = args.Binding;
        var bindB = obj.GetBinding();

        if (bindB is not null && isBindingProp)
        {
            var boxA = bindA.Dictionary.GetBox(param, memberType);
            var boxB = bindB.Dictionary.GetBox(member.Name, memberType);

            BoxTypeException.ThrowIfIsNotABox(boxA);
            BoxTypeException.ThrowIfIsNotABox(boxB);
            BoxValueTypeException.ThrowIfIsIncorrectType(boxA, memberType);
            BoxValueTypeException.ThrowIfIsIncorrectType(boxB, memberType);
            
            bindA.Dictionary.SetBox(param, boxB);
            return true;
        }

        bindA.Dictionary.SetBox(param, Box.CreateMember(memberType, member, obj));
        return true;
    }
}