using System;
using System.Reflection;
using System.Collections.Generic;

namespace Blindness;

public abstract class Stateness
{
    private List<State> states = new();

    protected abstract void load();
    public void Load()
    {
        findStates();
        load();
    }

    protected abstract void update();
    public void Update()
    {
        update();
        verify();
    }

    protected abstract void use();
    public void Use()
        => use();
    
    private void findStates()
    {
        var type = this.GetType();
        var fields = type.GetRuntimeFields();
        
        foreach (var field in fields)
            this.processField(field);
    }

    private void processField(FieldInfo field)
    {
        var fieldType = field.FieldType;
        if (!fieldType.IsValueType)
            return;
        
        
    }

    private void verify()
    {

    }
}