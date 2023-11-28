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
        try
        {
            foreach (var binding in bindings)
            {
                var info = getBindingInformation(binding);
                
                // TODO
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    internal void LoadMembers()
    {
        var type = this.GetType();
        foreach (var prop in type.GetRuntimeProperties())
            loadProperty(prop);
    }
    protected internal virtual void Load() { }

    private void loadProperty(PropertyInfo prop)
    {
        try
        {
            var get = prop.GetMethod;
            var set = prop.SetMethod;
            if (!get.IsVirtual || !set.IsVirtual)
                return;
            
            if (prop.PropertyType.IsSubclassOf(typeof(Node)))
            {
                initSubNode(prop);
                return;
            }
        }
        catch (Exception ex)
        {
            throw new PropertyInitializationException(
                prop.Name, ex
            );
        }
    }

    private void initSubNode(PropertyInfo prop)
    {
        var type = prop.PropertyType;

        if (type == prop.DeclaringType)
            throw new DependenceOverflowException(type);

        var node = DependencySystem
            .Current.GetConcrete(type);
        prop.SetValue(this, node);
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
            case ExpressionType.Constant:
                return null;
        }
        
        throw new InvalidBindingFormatException();
    }
}

public abstract class Node<T> : Node
    where T : Node<T>
{
    public static T Get()
        => DependencySystem.Current.GetConcrete<T>();

    public static T operator |(
        Node<T> node, Expression<Func<object, object>> binding)
    {
        node.Bind(binding);
        return node as T;
    }
}