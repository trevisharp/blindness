using System;
using System.Reflection;
using System.Collections.Generic;

namespace Blindness;

public abstract class Stateness
{
    private List<State> states = new();
}