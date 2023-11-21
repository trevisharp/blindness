using System;

namespace Blindness;

public class State
{
    public Stateness Parent { get; set; }
    public string Name { get; set; }
    public bool IsProperty { get; set; }
    public Type Type { get; set; }
}