/* Author:  Leonardo Trevisan Silio
 * Date:    23/07/2024
 */
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Blindness.Injection;

public class TypeFilterCollection : TypeFilter, IList<TypeFilter>
{
    readonly List<TypeFilter> filters = [];

    public override bool Filter(Type type)
        => filters.All(f => f.Filter(type));

    public TypeFilter this[int index]
    {
        get => filters[index]; 
        set => filters[index] = value;
    }

    public int Count => filters.Count;

    public bool IsReadOnly => false;

    public void Add(TypeFilter item)
        => filters.Add(item);

    public void Clear()
        => filters.Clear();

    public bool Contains(TypeFilter item)
        => filters.Contains(item);

    public void CopyTo(TypeFilter[] array, int arrayIndex)
        => filters.CopyTo(array, arrayIndex);

    public IEnumerator<TypeFilter> GetEnumerator()
        => filters.GetEnumerator();

    public int IndexOf(TypeFilter item)
        => filters.IndexOf(item);

    public void Insert(int index, TypeFilter item)
        => filters.Insert(index, item);

    public bool Remove(TypeFilter item)
        => filters.Remove(item);

    public void RemoveAt(int index)
        => filters.RemoveAt(index);

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();
}