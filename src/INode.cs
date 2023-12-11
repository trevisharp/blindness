using System;
using System.Linq.Expressions;

namespace Blindness;

public interface INode
{
    Binding Bind { get; set; }
    void Process();
    void When(
        Expression<Func<bool>> condition,
        Action action
    );
    void On(
        Expression<Func<bool>> condition,
        Action action
    );
}