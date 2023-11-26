using System;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace Blindness;

public abstract class Stateness
{

}

public abstract class Stateness<T> : Stateness
    where T : Stateness<T>
{
    public static T Get(params Func<dynamic, dynamic>[] deps)
        => DependencySystem.Current.GetConcrete<T>(deps);

    private List<State> states = new();

    // TODOs
    // -Identify recived fields has a parent state
    // -Try remove unused states
    private void loadStates()
    {
        var type = this.GetType();

        foreach (var prop in type.GetRuntimeProperties())
        {
            this.states.Add(new() {
                Parent = this,
                Name = prop.Name,
                IsProperty = true, 
                Type = prop.PropertyType
            });
        }

        foreach (var field in type.GetRuntimeFields())
        {
            this.states.Add(new() {
                Parent = this,
                Name = field.Name,
                IsProperty = false, 
                Type = field.FieldType
            });
        }
    }
}