/* Author:  Leonardo Trevisan Silio
 * Date:    07/08/2024
 */
using System;
using System.Reflection;
using System.Linq.Expressions;

namespace Blindness.Bind.ChainLinks;

using Boxes;
using Exceptions;

/// <summary>
/// Bind a expression of type A => B.C ... .
/// </summary>
public class ExpressionBindChainLink : BindChainLink
{
    protected override BindingResult TryHandle(BindingArgs args)
    {
        throw new NotImplementedException();
    }
}