/* Author:  Leonardo Trevisan Silio
 * Date:    12/08/2024
 */
using System;
using System.Collections.Generic;

namespace Blindness.Core;

using Bind;
using Bind.Boxes;
using Injection;
using Concurrency;

using Core.Concurrencies;
using Blindness.Factory;

/// <summary>
/// The default structure to a Blindness app.
/// </summary>
public class DefaultAppBehaviour : AppBehaviour
{
    public Implementer Implementer { get; set; } = new DefaultImplementer();

    readonly Dictionary<string, Stack<IBox<INode>>> apps = [];
    Stack<IBox<INode>> currentStack;

    public override INode CurrentMainNode => currentStack?.Peek()?.Open();

    public override void Run<T>(params object[] parameters)
    {
        try
        {
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
                    
                    // TODO: Test and improve
                    DependencySystem.Shared.UpdateAssembly(args.NewAssembly);
                    foreach (var app in apps)
                    {
                        var stack = app.Value;
                        foreach (var box in stack)
                        {
                            var oldNode = box.Open();
                            var newNode = Node.Replace(oldNode.GetType(), oldNode as Node);
                            box.Place(newNode as INode);
                        }
                    }
                };

                void implement()
                {
                    Verbose.Info("Implementing Concrete Nodes...");
                    Implementer.Implement();
                }

                hotReload.AddAction(implement);
                implement();
            }
            
            InitMain<T>(parameters);

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
        currentStack?.Push(new ValueBox<INode>(node));
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
        => currentStack?.Pop()?.Open();

    public override void Push<T>(params object[] parameters)
    {
        var node = Node.New(typeof(T)) as INode;
        currentStack?.Push(new ValueBox<INode>(node));
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