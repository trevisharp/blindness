/* Author:  Leonardo Trevisan Silio
 * Date:    16/11/2026
 */
using System;

namespace Blindness.Core;

/// <summary>
/// Define the base class, which must inherit from Node, of the concrete classes generated
/// for a node interface and for the interfaces that inherit from it.
/// </summary>
[AttributeUsage(AttributeTargets.Interface)]
public class ConcreteBaseAttribute(Type baseType) : Attribute
{
    public Type BaseType => baseType;
}
