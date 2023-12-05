using System;
using System.Reflection;
using System.Linq.Expressions;

namespace Blindness;

using Internal;
using Exceptions;

public abstract class Node
{
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
    protected internal virtual void OnProcess() { }
    public void Process()
        => OnProcess();

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