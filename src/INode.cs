using System;
using System.Linq.Expressions;

namespace Blindness;

using States;

public interface INode
{
    Binding Bind { get; set; }
    void Start();
    void When(
        Func<bool> condition,
        Action action
    );
    void On(
        Expression<Func<bool>> condition,
        Action<bool> action
    );
}