/* Author:  Leonardo Trevisan Silio
 * Date:    06/08/2024
 */
using System.Linq.Expressions;
using System.Collections.ObjectModel;

namespace Blindness.Bind;

/// <summary>
/// Base type for all chain links of a chain of responsability
/// of Binding algorithm that analyzes Expressions and make the
/// data binding.
/// </summary>
public record BindingArgs(
    Expression Body,
    ReadOnlyCollection<ParameterExpression> Parameters,
    Binding Binding,
    object Parent,
    object ParentBox,
    BindChain Chain
);