/* Author:  Leonardo Trevisan Silio
 * Date:    22/07/2024
 */
using System;
using System.Collections.Generic;

namespace Blindness.Core;

using Bind;
using Reload;
using Injection;
using Concurrency;

/// <summary>
/// The default structure to a Blindness app.
/// </summary>
public class DefaultAppBehaviour : AppBehaviour
{
    readonly Dictionary<string, Stack<INode>> apps = [];
    Stack<INode> currentStack;

    public override INode CurrentMainNode => currentStack?.Peek();

    public override void Run<T>()
    {
        try
        {
            
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }

    public override void Open<T>()
    {
        var node = Node.New(typeof(T)) as INode;
        if (currentStack.Count > 0)
            currentStack?.Pop();
        currentStack?.Push(node);
    }

    public override void Clear()
        => currentStack?.Clear();

    public override void Create(string app)
    {
        if (apps.ContainsKey(app))
            throw new NotImplementedException();
        apps.Add(app, []);
    }

    public override void Move(string app)
    {
        if (!apps.TryGetValue(app, out var stack))
            throw new NotImplementedException();
        currentStack = stack;
    }

    public override INode Pop()
        => currentStack?.Pop();

    public override void Push<T>()
    {
        var node = Node.New(typeof(T)) as INode;
        currentStack?.Push(node);
    }
}