/* Author:  Leonardo Trevisan Silio
 * Date:    08/08/2024
 */
using System;
using System.Linq;
using System.Reflection;

namespace Blindness.Bind.Boxes;

using Exceptions;

/// <summary>
/// Represents a structure to box values.
/// </summary>
public class ExpressionBox<T>(MemberInfo member, Delegate instanciator, object[] extraArgsBoxes = null) : IBox<T>
{
    public bool IsReadonly => !member.IsSettable();

    public T Open()
    {
        try
        {
            var instance = instanciator.DynamicInvoke();
            object[] args = extraArgsBoxes?.Select(Box.Open)?.ToArray();
            return (T)member.GetData(instance, args);
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
        
        var instance = instanciator.DynamicInvoke();
        object[] args = extraArgsBoxes?.Select(Box.Open)?.ToArray();
        member.SetData(instance, newValue, args);
    }
}