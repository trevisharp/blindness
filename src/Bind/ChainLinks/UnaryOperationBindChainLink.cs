/* Author:  Leonardo Trevisan Silio
 * Date:    14/11/2026
 */
using System;
using System.Linq.Expressions;

namespace Blindness.Bind.ChainLinks;

using Boxes;

/// <summary>
/// Represents a bind chain link for conversions, !a and -a expressions.
/// </summary>
public class UnaryOperationBindChainLink : BindChainLink
{
    protected override BindingResult TryHandle(BindingArgs args)
    {
        ArgumentNullException.ThrowIfNull(args, nameof(args));
        ArgumentNullException.ThrowIfNull(args.Body, nameof(args.Body));

        if (args.Body is not UnaryExpression unary)
            return BindingResult.Unsuccesfull;

        if (unary.NodeType is not (
            ExpressionType.Convert or ExpressionType.ConvertChecked or
            ExpressionType.Not or ExpressionType.Negate or ExpressionType.NegateChecked))
            return BindingResult.Unsuccesfull;

        var inner = args.Chain.Handle(args with { Body = unary.Operand });
        if (!inner.Success)
            return BindingResult.Unsuccesfull;

        var innerType = Box.GetBoxType(inner.MainBox);
        var resultType = unary.Type;

        if (unary.NodeType is ExpressionType.Convert or ExpressionType.ConvertChecked
            && innerType == resultType)
            return inner;

        var forward = BuildUnary(innerType, resultType, p =>
            Expression.MakeUnary(unary.NodeType, p, resultType, unary.Method)
        );

        var backward = TryBuildUnary(resultType, innerType, p => unary.NodeType switch
        {
            ExpressionType.Convert or ExpressionType.ConvertChecked
                => Expression.Convert(p, innerType),
            ExpressionType.Not when unary.Method is null
                => Expression.Not(p),
            ExpressionType.Negate or ExpressionType.NegateChecked when unary.Method is null
                => Expression.Negate(p),
            _ => null
        });

        return BindingResult.Successful(
            Box.CreateUnary(inner.MainBox, forward, backward)
        );
    }

    static Delegate BuildUnary(Type from, Type to, Func<Expression, Expression> body)
    {
        var param = Expression.Parameter(from);
        var funcType = typeof(Func<,>).MakeGenericType(from, to);
        return Expression.Lambda(funcType, body(param), param).Compile();
    }

    static Delegate TryBuildUnary(Type from, Type to, Func<Expression, Expression> body)
    {
        try
        {
            var param = Expression.Parameter(from);
            var expression = body(param);
            if (expression is null || expression.Type != to)
                return null;

            var funcType = typeof(Func<,>).MakeGenericType(from, to);
            return Expression.Lambda(funcType, expression, param).Compile();
        }
        catch (InvalidOperationException) { return null; }
        catch (ArgumentException) { return null; }
    }
}
