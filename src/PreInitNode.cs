using System;

namespace Blindness;

public class PreInitNode : Node
{
    public Type RealType { get; set; }
    public int DataIndex { get; set; }

    public static PreInitNode Create(Type type)
    {
        var preInitNode = new PreInitNode();
        int preInitNodeIndex = BindingSystem
            .Current.Add(preInitNode);
        preInitNode.RealType = type;
        preInitNode.DataIndex = preInitNodeIndex;
        return preInitNode;
    }
}