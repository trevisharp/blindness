/* Author:  Leonardo Trevisan Silio
 * Date:    09/08/2024
 */
using System;
using System.Linq.Expressions;

namespace Blindness.Bind.ChainLinks;

using Boxes;

/// <summary>
/// Represents a bind chain link for a + b expressions.
/// </summary>
public class BinaryOperationBindChainLink : BindChainLink
{
    protected override BindingResult TryHandle(BindingArgs args)
    {
        var body = args.Body.RemoveTypeCast();
        if (body is not BinaryExpression bin)
            return BindingResult.Unsuccesfull;
        
        var subArgs1 = args with { Body = bin.Left };
        var res1 = args.Chain.Handle(subArgs1);
        if (!res1.Success)
            return BindingResult.Unsuccesfull;

        var subArgs2 = args with { Body = bin.Right };
        var res2 = args.Chain.Handle(subArgs2);
        if (!res2.Success)
            return BindingResult.Unsuccesfull;
        
        var opType = Box.GetBoxType(res1.MainBox);

        // Testing especial operations between strings
        var strType = typeof(string);
        if (Box.GetBoxType(res1.MainBox) == strType && Box.GetBoxType(res2.MainBox) == strType)
        {
            var stringBox = bin.NodeType switch
            {
                ExpressionType.Add =>
                    Box.CreateOperation(
                        res1.MainBox, res2.MainBox,
                        opType.BuildBinaryFunction((a, b) => 
                            Expression.Call(null, strType.GetMethod("Concat", [ strType, strType ]), a, b)),
                        null,
                        null
                    ),

                _ => throw new NotImplementedException(
                    $"The operation {bin.NodeType} is not suported between strings in 'BinaryOperationBindChainLink' class."
                )
            };
            return BindingResult.Successful(stringBox);
        }

        var box = bin.NodeType switch
        {
            ExpressionType.Add =>
                Box.CreateOperation(
                    res1.MainBox, res2.MainBox,
                    opType.BuildBinaryFunction(Expression.Add),
                    opType.BuildBinaryFunction(Expression.Subtract),
                    opType.BuildBinaryFunction(Expression.Subtract)
                ),
                
            ExpressionType.Subtract =>
                Box.CreateOperation(
                    res1.MainBox, res2.MainBox,
                    opType.BuildBinaryFunction(Expression.Subtract),
                    opType.BuildBinaryFunction(Expression.Add),
                    opType.BuildBinaryFunction((res, lef) => Expression.Subtract(lef, res))
                ),
            
            ExpressionType.Multiply =>
                Box.CreateOperation(
                    res1.MainBox, res2.MainBox,
                    opType.BuildBinaryFunction(Expression.Multiply),
                    opType.BuildBinaryFunction(Expression.Divide),
                    opType.BuildBinaryFunction(Expression.Divide)
                ),
            
            ExpressionType.Divide =>
                Box.CreateOperation(
                    res1.MainBox, res2.MainBox,
                    opType.BuildBinaryFunction(Expression.Divide),
                    opType.BuildBinaryFunction(Expression.Multiply),
                    opType.BuildBinaryFunction((res, lef) => Expression.Divide(lef, res))
                ),

            _ => throw new NotImplementedException(
                $"The operation {bin.NodeType} is not suported by 'BinaryOperationBindChainLink' class."
            )
        };
        return BindingResult.Successful(box);
    }
}