/* Author:  Leonardo Trevisan Silio
 * Date:    07/08/2024
 */
using System;
using System.Linq.Expressions;

namespace Blindness.Bind.ChainLinks;

using Boxes;

/// <summary>
/// Represents a bind chain link for constant expressions.
/// </summary>
public class ConstantBindChainLink : BindChainLink
{
    protected override BindingResult TryHandle(BindingArgs args)
    {
        ArgumentNullException.ThrowIfNull(args, nameof(args));
        ArgumentNullException.ThrowIfNull(args.Body, nameof(args.Body));

        var body = args.Body.RemoveTypeCast();
        if (body is not ConstantExpression constant)
            return BindingResult.Unsuccesfull;

        return BindingResult.Successful(
            Box.CreateConstant(constant.Value)
        );
    }
}