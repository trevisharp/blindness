/* Author:  Leonardo Trevisan Silio
 * Date:    07/08/2024
 */
using System;
using System.Reflection;

namespace Blindness.Bind.Boxes;

using Exceptions;

/// <summary>
/// Represents a structure to box values.
/// </summary>
public class MemberBox<T>(MemberInfo member, object instance) : IBox<T>
{
    public bool IsReadonly => !member.IsSettable();

    public T Open()
    {
        try
        {
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
        
        member.SetData(instance, newValue);
    }
}