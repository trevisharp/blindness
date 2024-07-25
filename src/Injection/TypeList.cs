/* Author:  Leonardo Trevisan Silio
 * Date:    25/07/2024
 */
using System;

namespace Blindness.Injection;

/// <summary>
/// A linkedlist of types that allow cut operation.
/// </summary>
public class TypeList
{
    TypeNode first = null;
    TypeNode last = null;

    /// <summary>
    /// Add a Type to a list and reciver his created node.
    /// </summary>
    public TypeNode Add(Type type)
    {
        TypeNode node = new(type);
        if (first is null)
            return first = last = node;

        node.Previous = last;
        last.Next = node;
        last = last.Next;
        return node;
    }

    /// <summary>
    /// Remove the node and all of the nexts nodes.
    /// </summary>
    public void Cut(TypeNode node)
    {
        last = node.Previous;
        node.Previous = null;
        last.Next = null;
    }
}

/// <summary>
/// Represents a node for a linked list of types.
/// </summary>
public class TypeNode(Type value)
{
    public Type Value => value;
    public TypeNode Next { get; set; }
    public TypeNode Previous { get; set; }
}