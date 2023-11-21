using System;
using System.Reflection;
using System.Collections.Generic;

namespace Blindness;

public abstract class Stateness
{
    private List<State> states = new();
    private List<Stateness> children = new();

    public Stateness(params object[] deps)
    {

    }

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