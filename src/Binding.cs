using System;

namespace Blindness;

public class Binding
{
    public Node Parent { get; set; }
    public string Name { get; set; }
    public bool IsProperty { get; set; }
    public Type Type { get; set; }
}