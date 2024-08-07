/* Author:  Leonardo Trevisan Silio
 * Date:    07/08/2024
 */
using System;
using System.Linq.Expressions;

namespace Blindness.Bind.ChainLinks;

using Boxes;
using Exceptions;

/// <summary>
/// Represents a bind chain link for constant expressions.
/// </summary>
public class ConstantBindChainLink : BindChainLink
{
    protected override bool TryHandle(BindingArgs args, out BindingResult result)
    {
        ArgumentNullException.ThrowIfNull(args.Binding, nameof(args.Binding));

        result = new();
        var body = args.Body.RemoveTypeCast();
        if (body is not ConstantExpression constant)
            return false;
        
        if (args.Parameters.Count > 1)
            throw new WrongFormatBindException("A => B.C", "(A, B, ...) => W.Z");
        
        if (args.Parameters.Count == 0)
            throw new WrongFormatBindException("A => B.C", "() => B.C");

        var box = Box.CreateConstant(constant.Value);
        result.MainBox = box;
        return true;
    }
}