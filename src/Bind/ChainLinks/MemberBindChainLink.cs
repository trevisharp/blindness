/* Author:  Leonardo Trevisan Silio
 * Date:    02/08/2024
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
public class MemberBindChainLink : BindChainLink
{
    protected override bool TryHandle(BindingArgs args, out BindingResult result)
    {
        throw new NotImplementedException();
    }
}