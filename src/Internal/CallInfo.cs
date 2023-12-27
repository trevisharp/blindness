using System;
using System.Reflection;

namespace Blindness.Internal;

internal class CallInfo
{
    public Assembly Assembly { get; set; }
    public object Object { get; set; }
    public Type Type { get; set; }
    public MethodInfo Method { get; set; }

    public void Call(params object[] input)
    {
        this.Method.Invoke(
            this.Object,
            input
        );
    }
}