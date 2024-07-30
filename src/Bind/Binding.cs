/* Author:  Leonardo Trevisan Silio
 * Date:    30/07/2024
 */
using System;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace Blindness.Bind;

/// <summary>
/// A object to manage binding between boxes.
/// </summary>
public class Binding(object parent)
{

    public static Binding operator +(Binding binding, Expression<Func<object, object>> expression)
    {
        var fieldName = expression.Parameters[0].Name;
        Verbose.Warning(fieldName);
        Verbose.Warning(expression.Body);

        return binding;
    }
}