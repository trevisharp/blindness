/* Author:  Leonardo Trevisan Silio
 * Date:    12/08/2024
 */
using System;
using System.Collections.Generic;

namespace Blindness.Core;

using Bind;
using Reload;
using Injection;
using Concurrency;

using Core.Concurrencies;

/// <summary>
/// The default structure to a Blindness app.
/// </summary>
public class DefaultAppBehaviour : AppBehaviour
{
    readonly Dictionary<string, Stack<INode>> apps = [];
    Stack<INode> currentStack;

    public override INode CurrentMainNode => currentStack?.Peek();

    public override void Run<T>(params object[] parameters)
    {
        try
        {
            InitMain<T>(parameters);

            var model = new DefaultModel();

            if (App.Debug)
            {
                var hotReload = new HotReload(model);
                model.Run(hotReload);

                hotReload.OnSignal += (el, sa) =>
                {
                    if (sa is not AssemblySignalArgs args)
                        return;

                    if (!args.Success)
                        return;
                    
                    // HotReload Here!!
                };
            }

            var runner = new NodeRunner(model, () => CurrentMainNode);
            model.Run(runner);

            model.Start();
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }

    public override void Open<T>(params object[] parameters)
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

    public override void MoveTo(string app)
    {
        if (!apps.TryGetValue(app, out var stack))
            throw new NotImplementedException();
        currentStack = stack;
    }

    public override INode Pop()
        => currentStack?.Pop();

    public override void Push<T>(params object[] parameters)
    {
        var node = Node.New(typeof(T)) as INode;
        currentStack?.Push(node);
    }

    void InitMain<T>(params object[] parameters)
        where T : INode
    {
        if (!apps.ContainsKey("main"))
            Create("main");
        MoveTo("main");
        Open<T>(parameters);
    }
}