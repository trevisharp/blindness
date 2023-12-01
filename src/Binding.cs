using System;
using System.Linq.Expressions;

namespace Blindness;

public class Binding
{
    INode node;
    public Binding(INode node)
        => this.node = node;
    
    public T Get<T>(int fieldCode)
    {
        
    }

    public void Set<T>(int fieldCode, T value)
    {

    }

    public static Binding operator |(
        Binding binding, 
        Expression<Func<object, object>> exp
    )
    {
        throw new NotImplementedException();
    }
}