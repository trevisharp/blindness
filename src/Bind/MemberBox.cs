/* Author:  Leonardo Trevisan Silio
 * Date:    05/08/2024
 */
using System;
using System.Reflection;

namespace Blindness.Bind;

/// <summary>
/// Represents a structure to box values.
/// </summary>
public class MemberBox<T>(MemberInfo member, object instance) : IBox<T, T>
{   
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
        try
        {
            member.SetData(instance, newValue);
        }
        catch (Exception ex)
        {
            throw new Exception("An error was thrown on place box operation.", ex);
        }
    }
}