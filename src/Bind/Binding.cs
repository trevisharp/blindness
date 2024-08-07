/* Author:  Leonardo Trevisan Silio
 * Date:    07/08/2024
 */
using System;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace Blindness.Bind;

using Exceptions;

/// <summary>
/// A object to manage binding between boxes.
/// </summary>
public class Binding
{
    static readonly BindingCache cache = new(); 
    static IBindBehaviour behaviour = new DefaultBindBehaviour();
    static BindChain chain = null;
    
    /// <summary>
    /// Get the chain of behaviour to bind operations.
    /// </summary>
    public static BindChain Chain => 
        chain ??= behaviour.BuildChain();
    
    /// <summary>
    /// Set the behaviour of a bind operation.
    /// </summary>
    public static void SetBehaviour(IBindBehaviour newBehaviour)
    {
        ArgumentNullException.ThrowIfNull(newBehaviour, nameof(newBehaviour));
        behaviour = newBehaviour;
        chain = null;
    }

    public static void Bind(Expression<Func<bool>> expression)
    {
        ArgumentNullException.ThrowIfNull(expression, nameof(expression));
        var body = expression.Body;

        if (body is not BinaryExpression bin)
            throw new InvalidBindingFormatException("Expected: Bind(() => a.Prop == value);");
        
        if (bin.NodeType != ExpressionType.Equal)
            throw new InvalidBindingFormatException("Expected: Bind(() => a.Prop == value);");

        if (bin.Left is not MemberExpression mexp)
            throw new InvalidBindingFormatException("Expected: Bind(() => a.Prop == value);");
        
        var (obj, member) = mexp.SplitMember();
        var binding = cache[obj];
        
        
    }

    public BoxDictionary<string> Dictionary => dictionary;
    readonly BoxDictionary<string> dictionary = new();

    /// <summary>
    /// Get a box and open your value based on a key.
    /// </summary>
    public static T Open<T>(object obj, string boxName)
        => cache[obj].dictionary.Open<T>(boxName);

    /// <summary>
    /// Get a box and place your value based on a key.
    /// </summary>
    public static void Place<T>(object obj, string boxName, T value)
        => cache[obj].dictionary.Place(boxName, value);
    
    /// <summary>
    /// Get if the binding contains a box with a specific name.
    /// </summary>
    public static bool Contains(object obj, string boxName)
        => cache[obj].dictionary.Contains(boxName);
}