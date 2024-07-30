/* Author:  Leonardo Trevisan Silio
 * Date:    30/07/2024
 */
using System.Collections.Generic;

namespace Blindness.Bind;

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
        var box = GetOrInit<T>(boxName);
        return box.Open();
    }

    /// <summary>
    /// Place a value in a box and create it if needed.
    /// </summary>
    public void Place<T>(K boxName, T value)
    {
        var box = GetOrInit<T>(boxName);
        box.Place(value);
    }

    Box<T> GetOrInit<T>(K boxName)
    {
        if (memory.TryGetValue(boxName, out object obj))
            return obj as Box<T>;
        
        var box = new Box<T>();
        memory.Add(boxName, box);
        return box;
    }
}