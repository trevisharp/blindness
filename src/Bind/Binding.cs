/* Author:  Leonardo Trevisan Silio
 * Date:    07/08/2024
 */
using System;
using System.Linq.Expressions;

namespace Blindness.Bind;

using Behaviors;
using Analyzers;
using Exceptions;

/// <summary>
/// A object to manage binding between boxes.
/// </summary>
public class Binding
{
    static readonly BindingCache cache = new(); 
    static IBindAnalyzer leftAnalyzer = new DefaultLeftBindAnalyzer();
    static IBindAnalyzer rightAnalyzer = new DefaultRightBindAnalyzer();
    static BindChain leftChain = null, rightChain = null;
    static IBindBehavior behavior = new DefaultBindBehavior();
    
    /// <summary>
    /// Get the left chain of behaviour to bind operations.
    /// </summary>
    public static BindChain LeftChain => 
        leftChain ??= leftAnalyzer.BuildChain();

    /// <summary>
    /// Get the right chain of behaviour to bind operations.
    /// </summary>
    public static BindChain RightChain => 
        rightChain ??= rightAnalyzer.BuildChain();
    
    /// <summary>
    /// Set the behaviour of a bind operation.
    /// </summary>
    public static void SetBehaviour(
        IBindAnalyzer leftBehaviour,
        IBindAnalyzer rightBehaviour,
        IBindBehavior bindBehavior)
    {
        ArgumentNullException.ThrowIfNull(leftBehaviour, nameof(leftBehaviour));
        ArgumentNullException.ThrowIfNull(rightBehaviour, nameof(rightBehaviour));

        leftAnalyzer = leftBehaviour;
        rightAnalyzer = rightBehaviour;
        behavior = bindBehavior;

        leftChain = rightChain = null;
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
        
        var leftResult = LeftChain.Handle(new(bin.Left, LeftChain));
        var rightResult = RightChain.Handle(new(bin.Right, RightChain));

        if (!leftResult.Success || !rightResult.Success)
            throw new InvalidBindingException();
        
        behavior.MakeBinding(leftResult, rightResult);
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