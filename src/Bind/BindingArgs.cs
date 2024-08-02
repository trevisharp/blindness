/* Author:  Leonardo Trevisan Silio
 * Date:    02/08/2024
 */
using System;
using System.Linq.Expressions;

namespace Blindness.Bind;

/// <summary>
/// Base type for all chain links of a chain of responsability
/// of Binding algorithm that analyzes Expressions and make the
/// data binding.
/// </summary>
public record BindingArgs(
    Expression<Func<object, object>> Expression,
    Binding Binding,
    object Parent,
    object ParentBox
);