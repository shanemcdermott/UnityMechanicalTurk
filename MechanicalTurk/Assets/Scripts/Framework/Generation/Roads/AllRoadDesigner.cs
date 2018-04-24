using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllRoadDesigner : RoadDesigner
{

    public override void GetRoads(GridNode rootNode, ref Dictionary<Vector2Int, bool> roads)
    {
        List<Node> vertices = rootNode.GetChildVertices();
        //draw connections between verts

        foreach (Node node in vertices)
        {
            node.GetConnectionLines(ref roads);
        }

        if (bShouldDrawFromCenter)
        {
            DrawRoadsFromCenters(rootNode, ref roads);
        }
    }
}
