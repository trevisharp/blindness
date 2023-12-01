using System;
using System.Linq.Expressions;

namespace Blindness;

public class Binding
{
    public static Binding operator |(
        Binding binding, 
        Expression<Func<object, object>> exp
    )
    {
        throw new NotImplementedException();
    }
}