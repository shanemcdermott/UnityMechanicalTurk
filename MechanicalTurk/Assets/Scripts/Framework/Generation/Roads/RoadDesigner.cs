using System.Collections.Generic;
using UnityEngine;

public class RoadDesigner : MonoBehaviour
{
    public bool bShouldDrawFromCenter = true;

    public virtual void GetRoads(GridNode rootNode, ref Dictionary<Vector2Int, bool> roads)
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

    public virtual void DrawRoadsFromCenters(GridNode rootNode, ref Dictionary<Vector2Int, bool> roads)
    {
        List<GridNode> faces;
        rootNode.GetLeaves(out faces);

        for (int i = 0; i < faces.Count; i++)
        {
            int a = 0;
            int b = 1;
            if (i % 4 < 2)
            {
                a = 3;
                b = 2;
            }

            GetRoadsFromLeaf(ref roads, faces[i], a, b);
            //int a = (i % 2 == 0)? 3 : 0;
            //int b = (i % 3 == 0)? 1 : 2; 


            //Vector3 midPoint = MathOps.Midpoint(faces[i].GetVertex(a).GetPosition(), faces[i].GetVertex(b).GetPosition());
            //faces[i].GetConnectionLine(ref connectionPoints, faces[i].GetPosition(), midPoint);
        }
    }

    protected void GetRoadsFromLeaf(ref Dictionary<Vector2Int, bool> connectionPoints, GridNode leaf, int a, int b)
    {
        Vector3 midPoint = MathOps.Midpoint(leaf.GetVertex(a).GetPosition(), leaf.GetVertex(b).GetPosition());
        Node.GetConnectionLine(ref connectionPoints, leaf.GetPosition(), midPoint);
    }
}
