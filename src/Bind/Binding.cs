/* Author:  Leonardo Trevisan Silio
 * Date:    31/07/2024
 */
using System;
using System.Reflection;
using System.Linq.Expressions;

namespace Blindness.Bind;

/// <summary>
/// A object to manage binding between boxes.
/// </summary>
public class Binding(object parent)
{
    readonly BoxDictionary<string> dictionary = new();
    readonly object parent = parent;
    readonly Type parentType = parent.GetType();

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
        var propName = expression.Parameters[0].Name;
        var property = binding.parentType.GetProperty(propName);
        var propType = property.PropertyType;
        var box = binding.dictionary.GetBox(propName, propType);

        Verbose.Success(Box.Open(box));
        Verbose.Warning(expression.Body);

        return binding;
    }
}