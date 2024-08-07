/* Author:  Leonardo Trevisan Silio
 * Date:    02/08/2024
 */
using System;
using System.Linq.Expressions;

namespace Blindness.Bind;

using Exceptions;

/// <summary>
/// A object to manage binding between boxes.
/// </summary>
public class Binding(object parent)
{
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

    public BoxDictionary<string> Dictionary => dictionary;
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
    
    /// <summary>
    /// Get if the binding contains a box with a specific name.
    /// </summary>
    public bool Contains(string boxName)
        => dictionary.Contains(boxName);

    public static Binding operator |(Binding binding, Expression<Func<object, object>> expression)
    {
        ArgumentNullException.ThrowIfNull(binding, nameof(binding));
        ArgumentNullException.ThrowIfNull(expression, nameof(expression));

        if (binding.parent is null)
            throw new ParentNullBindException();

        var boxName = expression.Parameters[0].Name;
        var property = binding.parentType.GetProperty(boxName) 
            ?? throw new MissingPropertyBindException(boxName, binding.parentType);
        var propType = property.PropertyType;
        var box = binding.dictionary.GetBox(boxName, propType);

        var args = new BindingArgs(
            expression.Body,
            expression.Parameters,
            binding,
            binding.parent,
            box,
            Chain
        );
        var handled = Chain.Handle(args, out var result);
        
        if (!handled)
            throw new InvalidBindingException();

        

        return binding;
    }
}