/* Author:  Leonardo Trevisan Silio
 * Date:    09/08/2024
 */
using System;
using System.Linq.Expressions;

namespace Blindness.Bind.ChainLinks;

using Boxes;

/// <summary>
/// Represents a bind chain link for binary expressions like a + b or a == b.
/// </summary>
public class BinaryOperationBindChainLink : BindChainLink
{
    protected override BindingResult TryHandle(BindingArgs args)
    {
        var body = args.Body.RemoveTypeCast();
        if (body is not BinaryExpression bin)
            return BindingResult.Unsuccesfull;

        var res1 = args.Chain.Handle(args with { Body = bin.Left });
        if (!res1.Success)
            return BindingResult.Unsuccesfull;

        var res2 = args.Chain.Handle(args with { Body = bin.Right });
        if (!res2.Success)
            return BindingResult.Unsuccesfull;

        var leftType = Box.GetBoxType(res1.MainBox);
        var rightType = Box.GetBoxType(res2.MainBox);

        var operation = BindExtension.BuildBinaryFunction(leftType, rightType, (a, b) =>
            Expression.MakeBinary(bin.NodeType, a, b, bin.IsLiftedToNull, bin.Method)
        );

        var reversible = bin.Method is null
            && leftType == rightType
            && bin.Type == leftType;
        if (!reversible)
            return BindingResult.Successful(
                Box.CreateOperation(res1.MainBox, res2.MainBox, operation)
            );

        Func<Expression, Expression, Expression> computeLeft, computeRight;
        switch (bin.NodeType)
        {
            case ExpressionType.Add:
                computeLeft = Expression.Subtract;
                computeRight = Expression.Subtract;
                break;

            case ExpressionType.Subtract:
                computeLeft = Expression.Add;
                computeRight = (res, left) => Expression.Subtract(left, res);
                break;

            case ExpressionType.Multiply:
                computeLeft = Expression.Divide;
                computeRight = Expression.Divide;
                break;

            case ExpressionType.Divide:
                computeLeft = Expression.Multiply;
                computeRight = (res, left) => Expression.Divide(left, res);
                break;

            default:
                return BindingResult.Successful(
                    Box.CreateOperation(res1.MainBox, res2.MainBox, operation)
                );
        }

        return BindingResult.Successful(
            Box.CreateOperation(
                res1.MainBox, res2.MainBox, operation,
                leftType.BuildBinaryFunction(computeLeft),
                leftType.BuildBinaryFunction(computeRight)
            )
        );
    }
}
