using System;
using System.Reflection;
using System.Collections.Generic;

namespace Blindness;

public abstract class Node
{
    public void Bind(string targetName, string fieldName)
    {
        throw new NotImplementedException();
    }

    public void BindValue(string targetName, object value)
    {
        throw new NotImplementedException();
    }

    public void Bind(params Func<dynamic, dynamic>[] binding)
    {
        throw new NotImplementedException();
    }

    public virtual void Load() { }
}

public abstract class Node<T> : Node
    where T : Node<T>
{
    public static T Get()
        => DependencySystem.Current.GetConcrete<T>();

    private List<Binding> bindings = new();

    // TODOs
    // -Identify recived fields has a parent state
    // -Try remove unused states
    private void loadStates()
    {
        var type = this.GetType();

        foreach (var prop in type.GetRuntimeProperties())
        {
            this.bindings.Add(new() {
                Parent = this,
                Name = prop.Name,
                IsProperty = true, 
                Type = prop.PropertyType
            });
        }

        foreach (var field in type.GetRuntimeFields())
        {
            this.bindings.Add(new() {
                Parent = this,
                Name = field.Name,
                IsProperty = false, 
                Type = field.FieldType
            });
        }
    }
}