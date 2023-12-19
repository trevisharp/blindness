using System;
using System.Threading;
using System.Reflection;
using System.Linq.Expressions;

namespace Blindness;

using Internal;
using Parallelism;

public abstract class Node : IAsyncElement
{
    private int signalCount = 0;
    private AutoResetEvent signal = new(false);
    private bool running = true;

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
        Expression<Func<bool>> condition,
        Action action
    )
    {
        
    }

    public void On(
        Expression<Func<bool>> condition,
        Action action
    )
    {
        
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