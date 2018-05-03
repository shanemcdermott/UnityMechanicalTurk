using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Framework.Collections;

namespace Algorithms.City
{

    public class RoadBlockDesigner : RoadDesigner
    {
        public bool wantsVerticalConnections = true;
        public bool wantsVerticalFaceConnections = true;
        public bool wantsHorizontalConnections = true;
        public bool wantsHorizontalFaceConnections = true;

        public override void GetRoads(GridNode rootNode, ref Dictionary<Vector2Int, bool> roads)
        {
            List<GridNode> faces;
            rootNode.GetLeaves(out faces);

            for (int i = 0; i < faces.Count; i++)
            {
                List<Node> verts = faces[i].GetVertices();
                foreach (Node vert in verts)
                {
                    if (wantsVerticalConnections)
                        vert.GetVerticalConnections(ref roads);
                    if (wantsHorizontalConnections)
                        vert.GetHorizontalConnections(ref roads);
                }

                if (wantsVerticalFaceConnections)
                {
                    faces[i].GetVerticalConnections(ref roads);
                }
                if (wantsHorizontalFaceConnections)
                {
                    faces[i].GetHorizontalConnections(ref roads);
                }
                if (wantsVerticalConnections)
                {
                    int a = 2;
                    int b = 3;
                    if (i % 4 < 2)
                    {
                        a = 0;
                        b = 1;
                    }
                    GetRoadsFromLeaf(ref roads, faces[i], a, b);

                }
                if (wantsHorizontalConnections)
                {
                    int a = 1;
                    int b = 3;
                    if (i % 4 < 2)
                    {
                        a = 0;
                        b = 2;
                    }
                    GetRoadsFromLeaf(ref roads, faces[i], a, b);

                }
            }
        }
    }
}