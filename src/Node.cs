using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Blindness;

using Exceptions;

public abstract class Node
{
    public void Bind(params Expression<Func<object, object>>[] bindings)
    {
        foreach (var binding in bindings)
        {
            var info = getBindingInformation(binding);
            
            // TODO
        }
    }

    private (MemberInfo member, object obj) getBindingInformation(
        Expression<Func<object, object>> binding
    )
    {
        if (binding.Parameters.Count != 1)
            throw new InvalidBindingFormatException();
        var reciver = binding.Parameters[0].Name;
        
        var body = binding.Body;
        var member = fieldObjectSearch(body);
        if (member != null)
            return (member, null);
        
        var func = binding.Compile();
        return (null, func(null));
    }

    private MemberInfo fieldObjectSearch(Expression body)
    {
        switch (body.NodeType)
        {
            case ExpressionType.Convert:
                var unaryExp = body as UnaryExpression;
                var operand = unaryExp.Operand;
                return fieldObjectSearch(operand);
            
            case ExpressionType.MemberAccess:
                var memberExp = body as MemberExpression;
                return memberExp.Member;
            
            case ExpressionType.ListInit:
                return null;
        }
        
        throw new InvalidBindingFormatException();
    }

    public virtual void Load() { }
}

public abstract class Node<T> : Node
    where T : Node<T>
{
    public static T Get()
        => DependencySystem.Current.GetConcrete<T>();

    // TODOs
    // -Identify recived fields has a parent state
    // -Try remove unused states
    private void loadStates()
    {
        // var type = this.GetType();

        // foreach (var prop in type.GetRuntimeProperties())
        // {
        //     this.bindings.Add(new() {
        //         Parent = this,
        //         Name = prop.Name,
        //         IsProperty = true, 
        //         Type = prop.PropertyType
        //     });
        // }

        // foreach (var field in type.GetRuntimeFields())
        // {
        //     this.bindings.Add(new() {
        //         Parent = this,
        //         Name = field.Name,
        //         IsProperty = false, 
        //         Type = field.FieldType
        //     });
        // }
    }
}