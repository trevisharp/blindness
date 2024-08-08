/* Author:  Leonardo Trevisan Silio
 * Date:    07/08/2024
 */
using System;
using System.Reflection;
using System.Linq.Expressions;

namespace Blindness.Bind.Boxes;

using Exceptions;

/// <summary>
/// Represents a structure to box values.
/// </summary>
public class ExpressionBox<T>(MemberInfo member, LambdaExpression instanciator) : IBox<T>
{
    public bool IsReadonly => !member.IsSettable();

    public T Open()
    {
        try
        {
            var instance = instanciator.Compile().DynamicInvoke();
            var data = member.GetData(instance);
            return (T)member.GetData(instance);
        }
        catch (Exception ex)
        {
            throw new Exception("An error was thrown on open box operation.", ex);
        }
    }

    public void Place(T newValue)
    {
        if (!member.IsSettable())
            throw new ReadonlyBoxException();
        
        var instance = instanciator.Compile().DynamicInvoke();
        member.SetData(instance, newValue);
    }
}