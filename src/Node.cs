using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Blindness;

using System.Diagnostics.Tracing;
using System.Runtime.Intrinsics.X86;
using Exceptions;

public abstract class Node
{
    MethodInfo setBindInfo = null;
    MethodInfo getBindInfo = null;
    MethodInfo getBindIndexInfo = null;
    bool firstOperate = true;

    internal void Bind(params Expression<Func<object, object>>[] bindings)
    {
        // try
        // {
            foreach (var binding in bindings)
            {
                var info = getBindingInformation(binding);
                System.Console.WriteLine(info);
                var index = baseGetBindIndex(
                    info.field.Replace("_", "")
                );

                if (index == -1)
                    throw new MissingFieldException(
                        info.field, this.GetType()
                    );

                if (info.member is null)
                {
                    int dataIndex = BindingSystem
                        .Current.Add(info.obj);
                    baseSetBind(index, dataIndex);
                    return;
                }
                
                var parentType = info.member.DeclaringType;
                var parentGetBind = findMethod("baseGetBind", parentType);
                var parentGetBindIndex = findMethod("baseGetBindIndex", parentType);
                var parentBindIndex = parentGetBindIndex.Invoke(
                    info.parent, new object[] { info.member.Name }
                );
                var parentDataIndex = parentGetBind.Invoke(
                    info.parent, new object[] { parentBindIndex }
                );
                baseSetBind(index, (int)parentDataIndex);
            }
        // }
        // catch (Exception ex)
        // {
        //     throw ex;
        // }
    }

    internal void LoadMembers()
    {
        var type = this.GetType();
        foreach (var prop in type.GetRuntimeProperties())
            loadProperty(prop);
    }
    protected internal virtual void OnLoad() { }
    protected internal virtual void OnProcess() { }
    public void Process()
    {
        if (firstOperate)
        {
            OnLoad();
            firstOperate = false;
        }
        OnProcess();
    }

    internal void baseSetBind(int index, int code)
    {
        if (setBindInfo is null)
            setBindInfo = findMethod("setBind");
        
        setBindInfo.Invoke(this, new object[] { index, code });
    }
    internal int baseGetBind(int index)
    {
        if (getBindInfo is null)
            getBindInfo = findMethod("getBind");
        
        return (int)getBindInfo.Invoke(this, new object[] { index });
    }
    internal int baseGetBindIndex(string field)
    {
        if (getBindIndexInfo is null)
            getBindIndexInfo = findMethod("getBindIndex");
        
        return (int)getBindIndexInfo.Invoke(this, new object[] { field });
    }

    private MethodInfo findMethod(string name, Type type = null)
    {
        type ??= this.GetType();
        foreach (var method in type.GetRuntimeMethods())
        {
            if (method.Name == name)
                return method;
        }
        return null;
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

        var preInitNode = PreInitNode.Create(type);
        int index = baseGetBindIndex(prop.Name);
        baseSetBind(index, preInitNode.DataIndex);
    }

    private (string field, MemberInfo member, object obj, object parent) getBindingInformation(
        Expression<Func<object, object>> binding
    )
    {
        if (binding.Parameters.Count != 1)
            throw new InvalidBindingFormatException();
        var reciver = binding.Parameters[0].Name;
        
        var body = binding.Body;
        var result = fieldObjectSearch(body);
        if (result != null && result.Value.member.DeclaringType.IsSubclassOf(typeof(Node)))
        {
            return (reciver, result.Value.member, null, result.Value.obj);
        }
        
        var func = binding.Compile();
        return (reciver, null, func(null), null);
    }

    private (MemberInfo member, object obj)? fieldObjectSearch(
        Expression body
    )
    {
        switch (body.NodeType)
        {
            case ExpressionType.Parameter:
                throw new InvalidBindingFormatException(
                    @"
                    This error may be thrwoed by a expression in
                    x => x format, for techinical motivations try to use _x => x
                    "
                );

            case ExpressionType.Convert:
                var unaryExp = body as UnaryExpression;
                var operand = unaryExp.Operand;
                return fieldObjectSearch(operand);
            
            case ExpressionType.MemberAccess:
                var memberExp = body as MemberExpression;
                var consExp = memberExp.Expression as ConstantExpression;
                return (memberExp.Member, consExp.Value);
            
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
        if (node is null)
            return null;
        
        node.Bind(binding);
        return node as T;
    }
}