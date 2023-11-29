using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Blindness;

using Exceptions;

public abstract class Node
{
    MethodInfo setBindInfo = null;
    MethodInfo getBindInfo = null;
    MethodInfo getBindIndexInfo = null;

    internal void Bind(params Expression<Func<object, object>>[] bindings)
    {
        try
        {
            foreach (var binding in bindings)
            {
                var info = getBindingInformation(binding);
                var index = baseGetBindIndex(info.field);

                if (index == -1)
                    throw new MissingFieldException(
                        info.field, this.GetType()
                    );

                if (info.member is null)
                {
                    int dataIndex = BindingSystem
                        .Current.Add(info.obj);
                    baseSetBind(index, dataIndex);
                }
                
                var parentType = info.member.DeclaringType;
                var parentGetBind = parentType.GetRuntimeMethod(
                    "baseGetBind", new[] { typeof(int), typeof(int) }
                );
                var parentGetBindIndex = parentType.GetRuntimeMethod(
                    "baseGetBindIndex", new[] { typeof(string) }
                );
                var parentBindIndex = parentGetBindIndex.Invoke(
                    null, new object[] { info.member.Name }
                );
                var parentDataIndex = parentGetBind.Invoke(
                    null, new object[] { parentBindIndex }
                );
                baseSetBind(index, (int)parentDataIndex);
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

    void baseSetBind(int index, int code)
    {
        if (setBindInfo == null)
            setBindInfo = findMethod("setBind", typeof(int), typeof(int));
        
        setBindInfo.Invoke(this, new object[] { index, code });
    }
    int baseGetBind(int index)
    {
        if (getBindInfo == null)
            getBindInfo = findMethod("getBind", typeof(int));
        
        return (int)setBindInfo.Invoke(this, new object[] { index });
    }
    int baseGetBindIndex(string field)
    {
        if (getBindIndexInfo == null)
            getBindIndexInfo = findMethod("getBindIndex", typeof(string));
        
        return (int)setBindInfo.Invoke(this, new object[] { field });
    }

    private MethodInfo findMethod(string name, params Type[] types)
    {
        var type = this.GetType();
        var method = type.GetRuntimeMethod(name, types);
        return method;
    }

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

    private (string field, MemberInfo member, object obj) getBindingInformation(
        Expression<Func<object, object>> binding
    )
    {
        if (binding.Parameters.Count != 1)
            throw new InvalidBindingFormatException();
        var reciver = binding.Parameters[0].Name;
        
        var body = binding.Body;
        var member = fieldObjectSearch(body);
        if (member != null && member.DeclaringType.IsSubclassOf(typeof(Node)))
            return (reciver, member, null);
        
        var func = binding.Compile();
        return (reciver, null, func(null));
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
    {
        try
        {
            return DependencySystem.Current.GetConcrete<T>();
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public static T operator |(
        Node<T> node, Expression<Func<object, object>> binding)
    {
        node.Bind(binding);
        return node as T;
    }
}