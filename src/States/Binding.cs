/* Author:  Leonardo Trevisan Silio
 * Date:    15/07/2024
 */
using System;
using System.Linq;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace Blindness.States;

using Internal;
using Exceptions;
using Concurrency.Elements;

/// <summary>
/// Binding object no manage states.
/// </summary>
public class Binding(
    Node node, int fieldCount, Type parentType,
    Func<string, int> fieldMap)
{
    readonly Node node = node;
    readonly Func<string, int> fieldMap = fieldMap;
    readonly int[] pointerMap = Enumerable.Range(-1, fieldCount).ToArray();
    readonly List<EventElement>[] eventMap = new List<EventElement>[fieldCount];
    
    /// <summary>
    /// Get a value based on the code of the field.
    /// </summary>
    public T Get<T>(int fieldCode)
    {
        var pointer = GetBind(fieldCode);

        if (pointer == -1)
            pointer = tryInitField(typeof(T), fieldCode);

        return Memory.Current.Get<T>(pointer);
    }

    /// <summary>
    /// Set a value based on the code of the field.
    /// </summary>
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

        initFieldWithValue<T>(fieldCode, value);
    }
    
    internal void AddEvent(PropertyInfo prop, EventElement eventObj)
    {
        var index = fieldMap(prop.Name);
        var pointer = pointerMap[index];
        if (pointer == -1)
            pointer = tryInitField(prop.PropertyType, index);

        if (eventMap[index] is null)
            eventMap[index] = new();
        eventMap[index].Add(eventObj);

        Memory.Current.AddPointerListner(pointer, eventObj);
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
        
        var oldPointer = this.pointerMap[fieldCode];
        this.pointerMap[fieldCode] = pointer;

        var events = eventMap[fieldCode];
        if (events is null)
            return;
        foreach (var eventObj in events)
        {
            Memory.Current.RemovePointerListner(oldPointer, eventObj);
            Memory.Current.AddPointerListner(pointer, eventObj);
        }
    }

    public static Binding operator |(Binding binding, 
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

    private int tryInitField(Type fieldType, int fieldCode)
    {
        if (fieldType.GetInterface(nameof(INode)) == typeof(INode))
            throw new NonInitializatedNodeException(
                fieldType, this.node.GetType()
            );
        var isNullable = 
            fieldType.IsGenericType &&
            fieldType.GetGenericTypeDefinition() == typeof(Nullable<>);
        int newDataIndex = 
            fieldType == typeof(string) ?
            Memory.Current.Add(string.Empty) :
            fieldType.IsClass || isNullable ?
            Memory.Current.Add(null) :
            Memory.Current.Add(Activator.CreateInstance(fieldType));
        
        pointerMap[fieldCode] = newDataIndex;
        return newDataIndex;
    }
    
    private void bind(Expression<Func<object, object>> binding)
    {
        if (binding.Parameters.Count != 1)
            throw new InvalidBindingFormatException(
                "The number of parameters may be 1."
            );
        
        bool isBasic = tryBasicBind(binding);
        if (isBasic)
            return;
        
        throw new InvalidBindingFormatException();
    }

    private bool tryBasicBind(Expression<Func<object, object>> binding)
    {
        var bindInfo = FromToExpression
            .FromExpression(binding, node, parentType);
        
        if (bindInfo is null)
            return false;

        var toBinding = getBinding(
            bindInfo.To.ObjectValue
        );

        var fmFieldCode = this.fieldMap(
            bindInfo.From.MemberInfo.Name
        );
        var pointer = this.GetBind(fmFieldCode);
        if (pointer == -1)
            pointer = tryInitField(
                bindInfo.From.MemberInfo is PropertyInfo prop ? prop.PropertyType :
                bindInfo.From.MemberInfo is FieldInfo field ? field.FieldType :
                throw new InvalidBindingFormatException(),
                fmFieldCode
            );
        
        var toFieldCode = toBinding.fieldMap(
            bindInfo.To.MemberInfo.Name
        );
        toBinding.SetBind(toFieldCode, pointer);

        return true;
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

    private void initFieldWithValue<T>(int fieldCode, T value)
    {
        var newIndexData = Memory.Current.Add(value);
        this.pointerMap[fieldCode] = newIndexData;
    }
}