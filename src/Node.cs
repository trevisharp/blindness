using System;
using System.Threading;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace Blindness;

using States;
using Internal;
using Concurrency;
using Concurrency.Elements;

public abstract class Node : IAsyncElement
{
    private int signalCount = 0;
    private AutoResetEvent signal = new(false);
    private bool running = true;
    private List<(Func<bool> pred, Action act)> whenList = new();

    public Binding Bind { get; set; }
    public IAsyncModel Model { get; set; }
    public int MemoryLocation { get; set; } = -1;

    internal void LoadDependencies()
    {
        var deps = findDeps();
        if (deps is null)
            return;
        
        var parameters = deps.GetParameters();
        object[] objs = new object[parameters.Length];
        
        for (int i = 0; i < objs.Length; i++)
        {
            var param = parameters[i];
            var type = param.ParameterType;
            objs[i] = DependencySystem
                .Current.GetConcrete(type);
        }

        deps.Invoke(this, objs);
    }

    protected internal virtual void OnLoad() { }
    protected internal virtual void OnRun() { }

    public void Run()
        => Start();
    
    public void Start() 
    {
        this.running = true;
        OnRun();
        runWhenList();

        if (signalCount == 0)
            return;
        signal.Set();
    }

    public void Await()
    {
        signalCount++;
        signal.WaitOne();
        signalCount--;
    }

    public void Finish()
        => running = false;
    
    public void When(
        Func<bool> condition,
        Action action
    )
    {
        if (condition is null || action is null)
            return;
        
        whenList.Add((condition, action));
    }

    public void On(
        Expression<Func<bool>> condition,
        Action<bool> action
    )
    {
        EventElement eventElement = new EventElement(
            condition.Compile(), action
        );

        addEvents(condition, eventElement);

        Model.Run(eventElement);
        System.Console.WriteLine("Running...");
    }

    void addEvents(
        Expression exp,
        EventElement eventObj, 
        int depth = 0
    )
    {
        for (int i = 0; i < depth; i++)
            Console.Write('\t');
        Console.WriteLine(exp.NodeType);

        switch (exp.NodeType)
        {
            case ExpressionType.Lambda:
                var lambdaExp = exp as LambdaExpression;
                addEvents(lambdaExp.Body, eventObj, depth + 1);
                break;
        }
    }

    void addEvent(
        string parent,
        string field,
        EventElement eventObj
    )
    {

    }

    private void runWhenList()
    {
        foreach (var item in whenList)
        {
            if (item.pred())
                item.act();
        }
    }
    private MethodInfo findDeps()
        => findMethod("Deps");
    private MethodInfo findMethod(string name, Type type = null)
    {
        type ??= this.GetType();
        foreach (var method in type.GetRuntimeMethods())
        {
            if (method.Name != name)
                continue;
            
            return method;
        }
        return null;
    }
}