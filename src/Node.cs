using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Blindness;

public abstract class Node
{

    public virtual void Load() { }
}

public abstract class Node<T> : Node
    where T : Node<T>
{
    public static T Get()
        => DependencySystem.Current.GetConcrete<T>();
    public void Bind(params Expression<Func<T, object>>[] bindings)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        foreach (var binding in bindings)
        {
            
        }
        Console.ResetColor();
        throw new NotImplementedException();
    }

    // TODOs
    // -Identify recived fields has a parent state
    // -Try remove unused states
    private void loadStates()
    {
        // var type = this.GetType();

        // foreach (var prop in type.GetRuntimeProperties())
        // {
        //     this.bindings.Add(new() {
        //         Parent = this,
        //         Name = prop.Name,
        //         IsProperty = true, 
        //         Type = prop.PropertyType
        //     });
        // }

        // foreach (var field in type.GetRuntimeFields())
        // {
        //     this.bindings.Add(new() {
        //         Parent = this,
        //         Name = field.Name,
        //         IsProperty = false, 
        //         Type = field.FieldType
        //     });
        // }
    }
}