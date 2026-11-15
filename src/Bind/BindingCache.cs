/* Author:  Leonardo Trevisan Silio
 * Date:    07/08/2024
 */
using System;
using System.Runtime.CompilerServices;

namespace Blindness.Bind;

/// <summary>
/// A Cache to virtual associate objects to theiers bind without
/// affect the garbarage collector. Thread-safe.
/// </summary>
public class BindingCache
{
    readonly ConditionalWeakTable<object, Binding> table = new();

    public Binding this[object obj]
    {
        get
        {
            ArgumentNullException.ThrowIfNull(obj, nameof(obj));
            return table.GetValue(obj, static _ => new Binding());
        }
        set
        {
            ArgumentNullException.ThrowIfNull(obj, nameof(obj));
            ArgumentNullException.ThrowIfNull(value, nameof(value));
            table.AddOrUpdate(obj, value);
        }
    }
}
