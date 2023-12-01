using System;
using System.Linq.Expressions;

namespace Blindness;

public interface INode
{
    Binding Bind { get; set; }
    void Process();
}