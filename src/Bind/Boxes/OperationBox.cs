/* Author:  Leonardo Trevisan Silio
 * Date:    06/08/2024
 */
using System;
using System.Reflection;

namespace Blindness.Bind.Boxes;

/// <summary>
/// Represents a structure to box values.
/// </summary>
public class OperationBox<T>(IBox<T, T> boxA, IBox<T, T> boxB, MethodInfo operation) : IBox<T, T>
{   
    public T Open()
    {
        return default;
    }

    public void Place(T newValue)
    {
        
    }
}