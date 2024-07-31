/* Author:  Leonardo Trevisan Silio
 * Date:    31/07/2024
 */
using System;
using System.Collections.Generic;

namespace Blindness.Bind;

using Exceptions;

/// <summary>
/// A memory to map a key to a generic Box.
/// </summary>
public class BoxDictionary<K>
{
    readonly Dictionary<K, object> memory = [];

    /// <summary>
    /// Open a box and create it if needed.
    /// </summary>
    public T Open<T>(K boxName)
    {
        ArgumentNullException.ThrowIfNull(boxName, nameof(boxName));
        var box = GetOrCreate<T>(boxName);
        return box.Open();
    }

    /// <summary>
    /// Place a value in a box and create it if needed.
    /// </summary>
    public void Place<T>(K boxName, T value)
    {
        ArgumentNullException.ThrowIfNull(boxName, nameof(boxName));
        var box = GetOrCreate<T>(boxName);
        box.Place(value);
    }

    /// <summary>
    /// Get the box with a specific key. If the box not exists
    /// in the dictionary them create the box.
    /// </summary>
    public Box<T> GetOrCreate<T>(K boxName)
    {
        ArgumentNullException.ThrowIfNull(boxName, nameof(boxName));
        if (memory.TryGetValue(boxName, out object obj))
            return obj as Box<T>;
        
        var box = new Box<T>();
        memory.Add(boxName, box);
        return box;
    }

    /// <summary>
    /// Get the box object. Throws a exception if key do
    /// not found.
    /// </summary>
    public object GetBox(K boxName, Type boxType)
    {
        ArgumentNullException.ThrowIfNull(boxName, nameof(boxName));
        if (memory.TryGetValue(boxName, out object obj))
            return obj;
        
        var boxObj = Box.Create(boxType);
        memory.Add(boxName, boxObj);
        return boxObj;
    }

    /// <summary>
    /// Set the box object.
    /// </summary>
    public void SetBox(K boxName, object value)
    {
        ArgumentNullException.ThrowIfNull(boxName, nameof(boxName));
        BoxTypeException.ThrowIfIsNotABox(value);
        memory[boxName] = value;
    }
}