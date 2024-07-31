/* Author:  Leonardo Trevisan Silio
 * Date:    31/07/2024
 */
using System.Linq.Expressions;

namespace Blindness.Bind.ChainLinks;

public class SimpleMemberBindChainLink : BindChainLink
{
    protected override bool TryHandle(BindingArgs args)
    {
        var exp = args.Expression;
        var body = exp.Body;
        if (body is not MemberExpression member)
            return false;

        Verbose.Info(member.Expression);
        Verbose.Info(member.Expression.GetType());
        Verbose.Info(member.Member);
        
        return true;
    }
}