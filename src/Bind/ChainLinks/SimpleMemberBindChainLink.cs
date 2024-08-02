/* Author:  Leonardo Trevisan Silio
 * Date:    31/07/2024
 */
using System.Reflection;
using System.Linq.Expressions;

namespace Blindness.Bind.ChainLinks;

/// <summary>
/// Bind a expression of type A => B.C.
/// </summary>
public class SimpleMemberBindChainLink : BindChainLink
{
    protected override bool TryHandle(BindingArgs args)
    {
        var exp = args.Expression;
        var body = exp.Body;
        if (body is not MemberExpression member)
            return false;
        
        if (member.Expression is not MemberExpression subMember)
            return false;
        var mainConstExp = subMember.Expression as ConstantExpression;
        var mainObj = mainConstExp.Value;

        var objMember = subMember.Member;
        var field = objMember as FieldInfo;
        var obj = field.GetValue(mainObj);

        var mainMember = member.Member;
        var property = mainMember as PropertyInfo;

        Verbose.Info(obj);
        Verbose.Info(member.Expression as MemberExpression);
        Verbose.Info(member.Expression.GetType());
        Verbose.Info(member.Member);
        
        return true;
    }
}