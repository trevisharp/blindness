using System;
using System.Reflection;

namespace Blindness.Internal;

internal class CallInfo
{
    public CallInfo(MethodInfo method)
    {
        var type = method.DeclaringType;

        this.Assembly = type.Assembly;
        this.Type = type;
        this.OriginalMethod = method;
        this.CurrentMethod = method;
    }

    public Assembly Assembly { get; set; }
    public Type Type { get; set; }
    public MethodInfo OriginalMethod { get; set; }
    public MethodInfo CurrentMethod { get; set; }

    public void Call(object obj, params object[] input)
    {
        this.CurrentMethod?.Invoke(obj, input);
    }
}