using System;
using System.Reflection;
using System.Linq.Expressions;

namespace Blindness;

using Internal;
using Exceptions;

public class Binding
{
    Node node;
    int[] pointerMap;
    Func<string, int> fieldMap;
    object parentRef;
    Type parentType;

    public Binding(
        Node node, int fieldCount, Type parentType,
        Func<string, int> fieldMap
    )
    {
        this.node = node;
        this.pointerMap = new int[fieldCount];
        for (int i = 0; i < fieldCount; i++)
            pointerMap[i] = -1;
        this.fieldMap = fieldMap;
        this.parentType = parentType;
    }
    
    public T Get<T>(int fieldCode)
    {
        var pointer = GetBind(fieldCode);

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
    
    internal int GetBind(int fieldCode)
    {
        if (fieldCode < 0 || fieldCode >= pointerMap.Length)
            throw new ArgumentOutOfRangeException(nameof(fieldCode));
        var pointer = this.pointerMap[fieldCode];

        return pointer;
    }

    internal void SetBind(int fieldCode, int pointer)
    {
        if (fieldCode < 0 || fieldCode >= pointerMap.Length)
            throw new ArgumentOutOfRangeException(nameof(fieldCode));
        this.pointerMap[fieldCode] = pointer;
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
    
    private void bind(Expression<Func<object, object>> binding)
    {
        try
        {
            var bindInfo = FromToExpression
                .FromExpression(binding, node, parentType);

            var toBinding = getBinding(
                bindInfo.To.ObjectValue
            );

            var fmFieldCode = this.fieldMap(
                bindInfo.From.MemberInfo.Name
            );
            var pointer = this.GetBind(fmFieldCode);
            if (pointer == -1)
                tryInitField(
                    bindInfo.From.MemberInfo is PropertyInfo prop ? prop.PropertyType :
                    bindInfo.From.MemberInfo is FieldInfo field ? field.FieldType :
                    throw new InvalidBindingFormatException(),
        	        fmFieldCode
                );
            pointer = this.GetBind(fmFieldCode);
            
            var toFieldCode = toBinding.fieldMap(
                bindInfo.To.MemberInfo.Name
            );
            toBinding.SetBind(toFieldCode, pointer);
        }
        catch
        {
            throw;
        }
    }

    private Binding getBinding(object obj)
    {
        var type = obj.GetType();
        var prop = type.GetProperty("Bind");
        if (prop is null)
            throw new InvalidBindingFormatException(
                "The binding object need has a Binding Property with name 'Bind'"
            );
        
        var value = prop.GetValue(obj) as Binding;
        if (value is null)
            throw new InvalidBindingFormatException(
                "The binding object need has a Binding Property with name 'Bind'"
            );
        
        return value;
    }
}