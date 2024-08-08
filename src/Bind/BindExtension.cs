/* Author:  Leonardo Trevisan Silio
 * Date:    08/08/2024
 */
using System;
using System.Linq;
using System.Reflection;
using System.Linq.Expressions;

namespace Blindness.Bind;

using Exceptions;

/// <summary>
/// A extension class to System.Linq.Expression, System.Type
/// and other types to handle expressions and reflection more easely.
/// </summary>
public static class BindExtension
{
    public static Delegate BuildBinaryFunction(
        this Type operationType,
        Func<Expression, Expression, BinaryExpression> operation)
    {
        var leftParam = Expression.Parameter(operationType);
        var rightParam = Expression.Parameter(operationType);
        var lambda = Expression.Lambda(operation(leftParam, rightParam), leftParam, rightParam);
        return lambda.Compile();
    }

    /// <summary>
    /// Returns if the member can be setted like a field, property
    /// or a get with set method.
    /// </summary>
    public static bool IsSettable(this MemberInfo member)
    {
        if (member is FieldInfo)
            return true;

        if (member is PropertyInfo prop && prop.GetSetMethod() is not null)
            return true;
        
        if (member is not MethodInfo method)
            return false;

        var setVersionName = method.Name
            .Replace("get", "set")
            .Replace("Get", "Set");
        var setVersion = member.DeclaringType
            .GetMethod(setVersionName);
        return setVersion is not null;
    }

    /// <summary>
    /// Get internal binding from a object.
    /// </summary>
    public static Binding GetBinding(this object obj)
        => TryGetDataByType(obj, out Binding data) ? data : null;

    /// <summary>
    /// Get internal binding from a object.
    /// </summary>
    public static bool SetBinding(this object obj, Binding value)
        => TrySetDataByType(obj, value);

    /// <summary>
    /// Find in properties and fields of a object by a member
    /// with type T and get your data.
    /// </summary>
    public static bool TryGetDataByType<T>(this object obj, out T data)
    {
        var type = obj.GetType();
        var targetType = typeof(T);

        var property = type.GetRuntimeProperties()
            .FirstOrDefault(p => p.PropertyType == targetType);
        if (property is not null)
        {
            data = (T)GetProperty(obj, property);
            return true;
        }

        var field = type.GetRuntimeFields()
            .FirstOrDefault(p => p.FieldType == targetType);
        if (field is not null)
        {
            data = (T)GetField(obj, field);
            return true;
        }

        data = default;
        return false;
    }
    
    /// <summary>
    /// Find in properties and fields of a object by a member
    /// with type T and get your data.
    /// </summary>
    public static bool TrySetDataByType<T>(this object obj, T data)
    {
        var type = obj.GetType();
        var targetType = typeof(T);

        var property = type.GetRuntimeProperties()
            .FirstOrDefault(p => p.PropertyType == targetType);
        if (property is not null)
        {
            SetProperty(obj, property, data);
            return true;
        }

        var field = type.GetRuntimeFields()
            .FirstOrDefault(p => p.FieldType == targetType);
        if (field is not null)
        {
            SetField(obj, field, data);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Try get a member data value with a specific name
    /// from a specific object.
    /// </summary>
    public static bool TryGetData(
        this object obj,
        string memberName,
        out object data
    )
    {
        var type = obj.GetType();

        var property = type.GetProperty(memberName);
        if (property is not null)
        {
            data = GetProperty(obj, property); 
            return true;
        }

        var field = type.GetField(memberName);
        if (field is not null)
        {
            data = GetField(obj, field);
            return true;
        }

        data = null;
        return false;
    }

    public static Type GetMemberReturnType(
        this MemberInfo member
    )
    {
        ArgumentNullException.ThrowIfNull(member, nameof(member));

        if (member is PropertyInfo prop)
            return prop.PropertyType;
        
        if (member is FieldInfo field)
            return field.FieldType;
        
        if (member is MethodInfo method)
            return method.ReturnType;
        
        throw new Exception("Invalid Member.");
    }

    /// <summary>
    /// Get data from a member based on a instance.
    /// </summary>
    public static object GetData(this MemberInfo member, object instance, object[] extraData = null)
    {
        ArgumentNullException.ThrowIfNull(member, nameof(member));

        if (member is PropertyInfo prop)
            return GetProperty(instance, prop);
        
        if (member is FieldInfo field)
            return GetField(instance, field);
        
        if (member is MethodInfo method)
            return GetMethod(instance, method, extraData);

        throw new Exception("Invalid Member.");
    }

    /// <summary>
    /// Set Get data from a member based on a instance.
    /// </summary>
    public static void SetData(this MemberInfo member, object instance, object value, object[] extraData = null)
    {
        ArgumentNullException.ThrowIfNull(member, nameof(member));

        if (member is PropertyInfo prop)
        {
            SetProperty(instance, prop, value);
            return;
        }
        
        if (member is FieldInfo field)
        {
            SetField(instance, field, value);
            return;
        }
        
        if (member is MethodInfo method)
        {
            SetMethod(instance, method, extraData, value);
            return;
        }

        throw new Exception("Invalid Member.");
    }

    /// <summary>
    /// Remove any type cast of a operation returning the operando of expression.
    /// Example: (object)person.Age becomes person.Age.
    /// </summary>
    public static Expression RemoveTypeCast(this Expression expression)
    {
        if (expression is not UnaryExpression op)
            return expression;
        
        if (op.NodeType is not ExpressionType.Convert or ExpressionType.ConvertChecked)
            return expression;
        
        return op.Operand;
    }
    
    static object GetProperty(object obj, PropertyInfo prop)
    {
        if (obj is null || prop is null)
            return null;
        var getMethod = prop.GetGetMethod() 
            ?? throw new InvalidMemberOperationException(obj, prop, "get");
        return getMethod.Invoke(obj, []);
    }

    static object GetField(object obj, FieldInfo field)
    {
        if (obj is null || field is null)
            return null;
        return field.GetValue(obj);
    }
    
    static void SetProperty(object obj, PropertyInfo prop, object value)
    {
        if (obj is null || prop is null)
            return;
        var getMethod = prop.GetSetMethod() 
            ?? throw new InvalidMemberOperationException(obj, prop, "set");
        getMethod.Invoke(obj, [value]);
    }

    static void SetField(object obj, FieldInfo field, object value)
    {
        if (obj is null || field is null)
            return;
        field.SetValue(obj, value);
    }

    static object GetMethod(object obj, MethodInfo method, object[] inputs)
        => method?.Invoke(obj, inputs ?? []);

    static void SetMethod(object obj, MethodInfo method, object[] inputs, object value)
    {
        var parentType = method.DeclaringType;
        var expectedSetName = method.Name
            .Replace("get", "set")
            .Replace("Get", "Set");
        var setMethod = parentType.GetMethod(expectedSetName);
        GetMethod(obj, setMethod, [ ..inputs, value ]);
    }
}