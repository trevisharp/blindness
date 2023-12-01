using System;
using System.Linq.Expressions;

namespace Blindness;

public interface INode
{
    dynamic Bind(Expression<Func<object, object>> binding);
    void Process();
}