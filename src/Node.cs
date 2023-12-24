using System;
using System.Linq;
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
    }

    void addEvents(
        Expression exp,
        EventElement eventObj
    )
    {
        List<EventMatch> matches = new();
        addEvents(exp, eventObj, matches);

        var uniqueMatches = matches
            .Where(m => m.Field is not null)
            .DistinctBy(m => m.Field);
        
        foreach (var match in uniqueMatches)
            addEvent(match);
    }

    void addEvents(
        Expression exp,
        EventElement eventObj,
        List<EventMatch> capturedEvents
    )
    {
        switch (exp.NodeType)
        {
            case ExpressionType.Lambda:
                var lambdaExp = exp as LambdaExpression;
                addEvents(lambdaExp.Body, eventObj, capturedEvents);
                break;
            
            case ExpressionType.AndAlso:
            case ExpressionType.OrElse:
            case ExpressionType.Add:
            case ExpressionType.Subtract:
            case ExpressionType.Multiply:
            case ExpressionType.Divide:
            case ExpressionType.And:
            case ExpressionType.Or:
            case ExpressionType.Equal:
            case ExpressionType.GreaterThan:
            case ExpressionType.GreaterThanOrEqual:
            case ExpressionType.LessThan:
            case ExpressionType.LessThanOrEqual:
                var binExp = exp as BinaryExpression;
                addEvents(binExp.Left, eventObj, capturedEvents);
                addEvents(binExp.Right, eventObj, capturedEvents);
                break;

            case ExpressionType.MemberAccess:
                var memberExp = exp as MemberExpression;

                var propExp = memberExp.Expression as ConstantExpression;
                if (propExp is not null)
                {
                    capturedEvents.Add(new(propExp.Value, memberExp.Member as PropertyInfo, eventObj));
                    break;
                }
                
                addEvents(memberExp.Expression, eventObj, capturedEvents);
                break;
        }
    }

    void addEvent(
        EventMatch match
    )
    {
        var binding = getBinding(match.Parent);

        binding.AddEvent(
            match.Field,
            match.EventObject
        );
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

    private Binding getBinding(object type)
    {
        if (type is null)
            return null;
        
        var node = type as Node;
        if (node is null)
            return null;
        
        return node.Bind;
    }
    
    private MethodInfo findMethod(string name, Type type = null)
    {
        type ??= this.GetType();

        var method =  type.GetRuntimeMethods()
            .FirstOrDefault(p => p.Name == name);
        
        return method;
    }

    private PropertyInfo findProperty(string name, Type type = null)
    {
        type ??= this.GetType();
        
        var prop = type.GetRuntimeProperties()
            .FirstOrDefault(p => p.Name == name);
        
        return prop;
    }
}