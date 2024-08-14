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

using Factory;
using Injection;
using Concurrency;

using Core.Binds;
using Core.Concurrencies;

/// <summary>
/// The default structure to a Blindness app.
/// </summary>
public class DefaultAppBehaviour : AppBehaviour
{
    public Implementer Implementer { get; set; } = new DefaultImplementer();
    public IAsyncModel Model { get; set; } = new DefaultModel();

    readonly Dictionary<string, Stack<IBox<INode>>> apps = [];
    Stack<IBox<INode>> currentStack;

    public override INode CurrentMainNode => 
        currentStack.Count == 0 ? null : currentStack?.Peek()?.Open();

    public override void Run<T>(params object[] parameters)
    {
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

                hotReload.AddAction(implement);
            }

            firstOpen(hotReload);

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
                }
            }
        }

        void implement()
        {
            Verbose.Info("Implementing Concrete Nodes...", 1);
            Implementer.Implement();
        }

        void firstOpen(HotReload hotReload)
        {
            try { Open<T>(); }
            catch (Exception ex) when (App.Debug)
            {
                Verbose.Warning(ex.Message, 1);
                hotReload.Force();
                Open<T>();
            }
            catch
            {
                throw;
            }
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

    void InitMain()
    {
        if (!apps.ContainsKey("main"))
            Create("main");
        MoveTo("main");
    }
}