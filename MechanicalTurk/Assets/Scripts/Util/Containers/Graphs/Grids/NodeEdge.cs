using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeEdge : BaseConnection<Node>
{
    public NodeEdge(Node from, Node to) : base(from, to)
    {
    }
}
