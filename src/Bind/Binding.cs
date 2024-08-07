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

    /// <summary>
    /// Bind two expressions values.
    /// </summary>
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
        var binding = Get(obj);
        var box = binding.dictionary.GetBox(member.Name, member.GetMemberReturnType());

        
    }

    /// <summary>
    /// Get and init if needed the binding associated to the object.
    /// </summary>
    public static Binding Get(object obj)
    {
        ArgumentNullException.ThrowIfNull(obj, nameof(obj));

        var hasBinding = obj.TryGetDataByType<Binding>(out var binding);

        if (!hasBinding)
            return cache[obj];
        
        if (binding is not null)
            return binding;
        
        binding = new Binding();
        obj.TrySetDataByType(binding);
        return binding;
    }

    public BoxDictionary<string> Dictionary => dictionary;
    readonly BoxDictionary<string> dictionary = new();

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
    
    /// <summary>
    /// Get if the binding contains a box with a specific name.
    /// </summary>
    public bool Contains(string boxName)
        => dictionary.Contains(boxName);
}