using System;

namespace Blindness.Exceptions;

public class NonInitializatedNodeException : Exception
{
    Type nodeType;
    Type parentType;
    public NonInitializatedNodeException(
        Type nodeType, Type parentType
    )
    {
        this.nodeType = nodeType;
        this.parentType = parentType;
    }

    public override string Message =>
    $$"""
    The field of node type '{{nodeType}}' in the node '{{parentType}}'
    do not has been initializated. Consider add a Deps function:
    public interface {{parentType}}
    {
        {{nodeType}} fieldName { get; set; }
        void Deps({{nodeType}} fieldName);
        
        // ...
    }

    Other possibility is your code using Binding in a null Node field, like this:
    public interface {{parentType}}
    {
        {{nodeType}} fieldName { get; set; } // Has a 'x' field Null
        OtherNodeType otherName { get; set; }

        void OnLoad()
        {
            fieldName.Bind |= x => otherName.y;
        }
    }
    The 'x' in fieldName do not have a reference to pass to 'y' in otherName. 
    """;
}