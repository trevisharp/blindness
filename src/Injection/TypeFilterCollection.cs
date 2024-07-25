/* Author:  Leonardo Trevisan Silio
 * Date:    23/07/2024
 */
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Blindness.Injection;

public class TypeFilterCollection : BaseTypeFilter, IList<BaseTypeFilter>
{
    readonly List<BaseTypeFilter> filters = [];

    public override bool Filter(Type type)
        => filters.All(f => f.Filter(type));

    public BaseTypeFilter this[int index]
    {
        get => filters[index]; 
        set => filters[index] = value;
    }

    public int Count => filters.Count;

    public bool IsReadOnly => false;

    public void Add(BaseTypeFilter item)
        => filters.Add(item);

    public void Clear()
        => filters.Clear();

    public bool Contains(BaseTypeFilter item)
        => filters.Contains(item);

    public void CopyTo(BaseTypeFilter[] array, int arrayIndex)
        => filters.CopyTo(array, arrayIndex);

    public IEnumerator<BaseTypeFilter> GetEnumerator()
        => filters.GetEnumerator();

    public int IndexOf(BaseTypeFilter item)
        => filters.IndexOf(item);

    public void Insert(int index, BaseTypeFilter item)
        => filters.Insert(index, item);

    public bool Remove(BaseTypeFilter item)
        => filters.Remove(item);

    public void RemoveAt(int index)
        => filters.RemoveAt(index);

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();
}