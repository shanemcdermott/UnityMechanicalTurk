using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeEdge : BaseConnection<Node>
{
    public NodeEdge(Node from, Node to) : base(from, to)
    {
    }

    public override float GetCost()
    {
        float scale = 1;
        if(fromNode != null && toNode != null)
        {
            scale = Vector3.Distance(fromNode.GetPosition(), toNode.GetPosition());
        }
        return base.GetCost() * scale;
    }
}
