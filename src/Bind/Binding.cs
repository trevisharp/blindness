/* Author:  Leonardo Trevisan Silio
 * Date:    30/07/2024
 */
using System;
using System.Linq.Expressions;

namespace Blindness.Bind;

/// <summary>
/// A object to manage binding between boxes.
/// </summary>
public class Binding<K>(object parent)
{
    readonly BoxDictionary<K> boxDictionary = new();

    /// <summary>
    /// Get a box and open your value based on a key.
    /// </summary>
    public T Open<T>(K boxKey)
        => boxDictionary.Open<T>(boxKey);

    /// <summary>
    /// Get a box and place your value based on a key.
    /// </summary>
    public void Place<T>(K boxKey, T value)
        => boxDictionary.Place(boxKey, value);

    public static Binding<K> operator +(Binding<K> binding, Expression<Func<object, object>> expression)
    {
        var fieldName = expression.Parameters[0].Name;
        Verbose.Warning(fieldName);
        Verbose.Warning(expression.Body);

        return binding;
    }
}