/* Author:  Leonardo Trevisan Silio
 * Date:    31/07/2024
 */
using System;
using System.Reflection;
using System.Linq.Expressions;

namespace Blindness.Bind;

using Exceptions;

/// <summary>
/// A object to manage binding between boxes.
/// </summary>
public class Binding(object parent)
{
    readonly BoxDictionary<string> dictionary = new();
    readonly object parent = parent;
    readonly Type parentType = parent?.GetType();

    /// <summary>
    /// Get a box and open your value based on a key.
    /// </summary>
    public T Open<T>(string boxName)
        => dictionary.Open<T>(boxName);

    /// <summary>
    /// Get a box and place your value based on a key.
    /// </summary>
    public void Place<T>(string boxName, T value)
        => dictionary.Place(boxName, value);

    public static Binding operator +(Binding binding, Expression<Func<object, object>> expression)
    {
        ArgumentNullException.ThrowIfNull(nameof(binding));
        ArgumentNullException.ThrowIfNull(nameof(expression));

        if (binding.parent is null)
            throw new ParentNullBindException();

        var boxName = expression.Parameters[0].Name;
        var property = binding.parentType.GetProperty(boxName) 
            ?? throw new MissingPropertyBindException(boxName, binding.parentType);
        var propType = property.PropertyType;
        var box = binding.dictionary.GetBox(boxName, propType);
        
        // TODO: 
        //  Verify considering the boxing operations
        //  The expression always returns a object
        //  if the return is a int, a automatic boxing is
        //  performed, so the expType is Object if the
        //  real type is not a reference type.
        // var expType = expression.Body.Type;
        // if (!Box.IsBoxType(box, expression.Body.Type))
        //     throw new TypeBindException(
        //         binding.parent, boxName, 
        //         propType, expType
        //     );

        Verbose.Warning(expression.Body);
        Verbose.Warning(expression.Body.Type);
        Verbose.Warning(expression.Body.NodeType);

        return binding;
    }
}