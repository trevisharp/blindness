/* Author:  Leonardo Trevisan Silio
 * Date:    14/08/2024
 */
using System;
using System.Collections.Generic;

namespace Blindness.Core;

using Bind;
using Bind.Boxes;
using Bind.Behaviors;
using Bind.Analyzers;

using Injection;
using Concurrency;

using Core.Binds;
using Core.Concurrencies;

/// <summary>
/// The default structure to a Blindness app.
/// </summary>
public class DefaultAppBehaviour : AppBehaviour
{
    public IAsyncModel Model { get; set; } = new DefaultModel();

    readonly Dictionary<string, Stack<IBox<INode>>> apps = [];
    Stack<IBox<INode>> currentStack;

    public override INode CurrentMainNode => 
        currentStack is null || currentStack.Count == 0 ? null : currentStack.Peek()?.Open();

    public override void Run<T>(params object[] parameters)
    {
        ThrowIfHasParameters(parameters);
        try
        {
            Binding.SetBehaviour(
                new DeepLeftBindAnalyzer(),
                new DefaultRightBindAnalyzer(),
                new DefaultBindBehavior()
            );
            
            InitMain();

            HotReload hotReload = null;
            if (App.Debug)
            {
                hotReload = new(Model);
                Model.Run(hotReload);
                
                hotReload.OnSignal += (el, sa) =>
                {
                    if (sa is not AssemblySignalArgs args)
                        return;

                    if (!args.Success)
                        return;
                    
                    hotreload(args);
                };
            }

            Open<T>();

            var runner = new NodeRunner(Model, () => CurrentMainNode as Node);
            Model.Run(runner);

            Model.OnError += (el, ex) =>
            {
                Verbose.Error($"Error on {el} element.");
                ShowError(ex);
            };

            Model.Start();
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }

        void hotreload(AssemblySignalArgs args)
        {      
            Verbose.Info("Applying Hot Reload...", 1);
            DependencySystem.Shared.UpdateAssembly(args.NewAssembly);
            
            foreach (var app in apps)
            {
                var stack = app.Value;
                foreach (var box in stack)
                {
                    var oldNode = box.Open();
                    var newNode = Node.Recreate(args.NewAssembly, oldNode as Node);
                    box.Place(newNode as INode);
                    oldNode?.Unload();
                }
            }
        }
    }

    public override void Open<T>(params object[] parameters)
    {
        ThrowIfHasParameters(parameters);
        var node = Node.New(typeof(T)) as INode;
        if (currentStack.Count > 0)
            currentStack?.Pop()?.Open()?.Unload();
        currentStack?.Push(new ValueBox<INode>(node));
    }

    public override void Clear()
    {
        if (currentStack is null)
            return;

        while (currentStack.Count > 0)
            currentStack.Pop()?.Open()?.Unload();
    }

    public override void Create(string app)
    {
        ArgumentNullException.ThrowIfNull(app, nameof(app));
        if (!apps.TryAdd(app, []))
            throw new ArgumentException($"The app '{app}' already exists.", nameof(app));
    }

    public override void MoveTo(string app)
    {
        ArgumentNullException.ThrowIfNull(app, nameof(app));
        if (!apps.TryGetValue(app, out var stack))
            throw new KeyNotFoundException($"The app '{app}' does not exist. Call Create first.");
        currentStack = stack;
    }

    public override INode Pop()
    {
        var node = currentStack?.Pop()?.Open();
        node?.Unload();
        return node;
    }

    public override void Push<T>(params object[] parameters)
    {
        ThrowIfHasParameters(parameters);
        var node = Node.New(typeof(T)) as INode;
        currentStack?.Push(new ValueBox<INode>(node));
    }

    /// <summary>
    /// Throws a exception if parameters are given, since nodes do not receive parameters yet.
    /// </summary>
    static void ThrowIfHasParameters(object[] parameters)
    {
        if (parameters is { Length: > 0 })
            throw new NotSupportedException(
                "Passing parameters to a node is not supported by DefaultAppBehaviour."
            );
    }

    void InitMain()
    {
        if (!apps.ContainsKey("main"))
            Create("main");
        MoveTo("main");
    }
}