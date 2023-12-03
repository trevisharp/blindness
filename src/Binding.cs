using System;
using System.Reflection;
using System.Linq.Expressions;

namespace Blindness;

using Exceptions;

public class Binding
{
    Node node;
    int[] pointerMap;
    Func<string, int> fieldMap;
    public Binding(
        Node node, int fieldCount, 
        Func<string, int> fieldMap
    )
    {
        this.node = node;
        this.pointerMap = new int[fieldCount];
        for (int i = 0; i < fieldCount; i++)
            pointerMap[i] = -1;
        System.Console.WriteLine(fieldMap("input"));
        this.fieldMap = fieldMap;
    }
    
    public T Get<T>(int fieldCode)
    {
        if (fieldCode < 0 || fieldCode >= pointerMap.Length)
            throw new ArgumentOutOfRangeException(nameof(fieldCode));
        var pointer = this.pointerMap[fieldCode];

        if (pointer == -1)
            tryInitField(typeof(T), fieldCode);

        return Memory.Current.Get<T>(pointer);
    }

    public void Set<T>(int fieldCode, T value)
    {
        if (fieldCode < 0 || fieldCode >= pointerMap.Length)
            throw new ArgumentOutOfRangeException(nameof(fieldCode));
        var pointer = this.pointerMap[fieldCode];
        
        if (pointer != -1)
        {
            Memory.Current.Set<T>(pointer, value);
            return;
        }

        var newIndexData = Memory.Current.Add(value);
        this.pointerMap[fieldCode] = newIndexData;
    }
    
    public static Binding operator |(
        Binding binding, 
        Expression<Func<object, object>> bindExpression
    )
    {
        if (binding is null)
            throw new ArgumentNullException(nameof(binding));
        if (bindExpression is null)
            throw new ArgumentNullException(nameof(bindExpression));
        
        binding.bind(bindExpression);
        return binding;
    }

    private void bind(Expression<Func<object, object>> binding)
    {
        var info = getBindingInformation(binding);
        System.Console.WriteLine(info.field);
        System.Console.WriteLine(info.field.Replace("_", ""));
        System.Console.WriteLine(fieldMap(info.field.Replace("_", "")));
        var index = fieldMap(
            info.field.Replace("_", "")
        );

        if (index == -1)
            throw new MissingFieldException(
                info.field, info.parent.GetType()
            );

        if (info.member is null)
            throw new InvalidBindingFormatException();
    
        // TODO: Reimplement
        var parentType = info.member.DeclaringType;
        
        Binding bindingObj = null;
        foreach (var field in parentType.GetRuntimeFields())
        {
            if (field.FieldType == typeof(Binding))
            {
                
            }
        }
        var parentGetBind = findMethod("baseGetBind", parentType);
        var parentGetBindIndex = findMethod("baseGetBindIndex", parentType);
        var parentBindIndex = parentGetBindIndex.Invoke(
            info.parent, new object[] { info.member.Name }
        );
        var parentDataIndex = parentGetBind.Invoke(
            info.parent, new object[] { parentBindIndex }
        );
        
        pointerMap[index] = (int)parentDataIndex;
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
        if (result != null && result.Value.member.DeclaringType.GetInterface("INode") != null)
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

    private MethodInfo findMethod(string name, Type type = null)
    {
        type ??= this.GetType();
        foreach (var method in type.GetRuntimeMethods())
        {
            if (method.Name != name)
                continue;
            
            return method;
        }
        return null;
    }

    private void tryInitField(Type fieldType, int fieldCode)
    {
        if (fieldType.GetInterface(nameof(INode)) == typeof(INode))
            throw new NonInitializatedNodeException(
                fieldType, this.node.GetType()
            );
        var isNullable = 
            fieldType.IsGenericType &&
            fieldType.GetGenericTypeDefinition() == typeof(Nullable<>);
        int newDataIndex = 
            fieldType.IsClass || isNullable ?
            Memory.Current.Add(null) :
            Memory.Current.Add(Activator.CreateInstance(fieldType));
        
        pointerMap[fieldCode] = newDataIndex;
    }
}