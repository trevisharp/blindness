/* Author:  Leonardo Trevisan Silio
 * Date:    08/08/2024
 */
using System;
using System.Linq;
using System.Reflection;
using System.Linq.Expressions;

namespace Blindness.Bind.Boxes;

using Exceptions;

public static class Box
{
    /// <summary>
    /// Create a ValueBox from a type.
    /// </summary>
    public static object Create(Type type)
    {
        var boxType = typeof(ValueBox<>);
        var genBoxType = boxType.MakeGenericType(type);
        var boxConstructor = genBoxType.GetConstructor([ type ]);
        var boxObj = boxConstructor.Invoke([ null ]);
        return boxObj;
    }
    
    /// <summary>
    /// Create a ExpressionBox from a type.
    /// </summary>
    public static object CreateExpression(
        Type type, MemberInfo member, 
        Delegate instanciator,
        object[] extraArgsBoxes)
    {
        var boxType = typeof(ExpressionBox<>);
        var genBoxType = boxType.MakeGenericType(type);
        var boxConstructor = genBoxType.GetConstructor([ 
            typeof(MemberInfo), typeof(Delegate), typeof(object[])
        ]);
        var boxObj = boxConstructor.Invoke([ member, instanciator, extraArgsBoxes ]);
        return boxObj;
    }

    /// <summary>
    /// Create a ConstantBox from a type.
    /// </summary>
    public static object CreateConstant(object value)
    {
        var boxType = typeof(ConstantBox<>);
        var genBoxType = boxType.MakeGenericType(value.GetType());
        var boxConstructor = genBoxType.GetConstructor([ value.GetType() ]);
        var boxObj = boxConstructor.Invoke([ value ]);
        return boxObj;
    }

    /// <summary>
    /// Create a OperationBox from a type.
    /// </summary>
    public static object CreateOperation(
        object leftBox, object rightBox,
        MethodInfo operation,
        object reverseLeft = null,
        object reverseRight = null)
    {
        var boxType = typeof(OperationBox<>);
        var T = operation.ReturnType;
        var genBoxType = boxType.MakeGenericType(T);
        var boxConstructor = genBoxType.GetConstructor([ 
            typeof(IBox<>).MakeGenericType(T, T),
            typeof(IBox<>).MakeGenericType(T, T),
            typeof(MethodInfo),
            typeof(Func<>).MakeGenericType(T, T, T),
            typeof(Func<>).MakeGenericType(T, T, T)
        ]);
        var boxObj = boxConstructor.Invoke([ 
            leftBox, rightBox, operation, 
            reverseLeft, reverseRight
        ]);
        return boxObj;
    }

    /// <summary>
    /// Create a InnerBox from a type.
    /// </summary>
    public static object CreateInner(object innerBox, Type innerType)
    {
        var boxType = typeof(InnerBox<>);
        var genBoxType = boxType.MakeGenericType(innerType);
        var boxConstructor = genBoxType.GetConstructor([ innerBox.GetType() ]);
        var boxObj = boxConstructor.Invoke([ innerBox ]);
        return boxObj;
    }

    /// <summary>
    /// Test if a object is a Box<T>
    /// </summary>
    public static bool IsBox(object value)
        => value.GetType().GetInterfaces().Any(i =>
            i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IBox<,>)
        );

    /// <summary>
    /// Test if a type is the type of internal data
    /// of a box object. If the object is not a box
    /// the function throws a exception.
    /// </summary>
    public static bool IsBoxType(object box, Type type)
    {
        ArgumentNullException.ThrowIfNull(box);
        ArgumentNullException.ThrowIfNull(type);
        BoxTypeException.ThrowIfIsNotABox(box);

        var genericParam = box.GetType().GetGenericArguments()[0];
        return genericParam == type;
    }

    /// <summary>
    /// Get if the box is readonly.
    /// </summary>
    public static bool IsReadOnly(object box)
    {
        ArgumentNullException.ThrowIfNull(box, nameof(box));
        BoxTypeException.ThrowIfIsNotABox(box);

        var boxType = box.GetType();
        var readonlyProperty = boxType.GetProperty("IsReadonly");
        var getIsReadonly = readonlyProperty.GetGetMethod();
        return (bool)getIsReadonly.Invoke(box, []);
    }

    /// <summary>
    /// Get the Inner property in the box is a InnerBox, otherside throws a error.
    /// </summary>
    public static object GetInner(object box)
    {
        var boxType = box.GetType();
        if (boxType.GetGenericTypeDefinition() != typeof(InnerBox<>))
            throw new BoxTypeException(box, "InnerBox");
        
        var innerProp = boxType.GetProperty("Inner") 
            ?? throw new BoxTypeException(box, "InnerBox");
        
        var innerGet = innerProp.GetGetMethod()
            ?? throw new BoxTypeException(box, "InnerBox");
        
        return innerGet.Invoke(box, []);
    }

    /// <summary>
    /// Set the Inner property in the box is a InnerBox, otherside throws a error.
    /// </summary>
    public static void SetInner(object box, object newInner)
    {
        var boxType = box.GetType();
        if (boxType.GetGenericTypeDefinition() != typeof(InnerBox<>))
            throw new BoxTypeException(box, "InnerBox");
        
        BoxTypeException.ThrowIfIsNotABox(newInner);
        
        var innerProp = boxType.GetProperty("Inner") 
            ?? throw new BoxTypeException(box, "InnerBox");
        
        var innerSet = innerProp.GetSetMethod()
            ?? throw new BoxTypeException(box, "InnerBox");
        
        innerSet.Invoke(box, [ newInner ]);
    }

    /// <summary>
    /// Try to open a object like a box. Throw a exception if
    /// the object is not a box.
    /// </summary>
    public static object Open(object box)
    {
        ArgumentNullException.ThrowIfNull(box, nameof(box));
        BoxTypeException.ThrowIfIsNotABox(box);

        var boxType = box.GetType();
        var placeMethod = boxType.GetMethod("Open");
        return placeMethod.Invoke(box, []);
    }
    
    /// <summary>
    /// Try to place a value in a object like a box. Throw a exception if
    /// the object is not a box.
    /// </summary>
    public static void Place(object box, object value)
    {
        ArgumentNullException.ThrowIfNull(box, nameof(box));
        BoxTypeException.ThrowIfIsNotABox(box);
        
        var boxType = box.GetType();
        var placeMethod = boxType.GetMethod("Place");
        placeMethod.Invoke(box, [ value ]);
    }
}