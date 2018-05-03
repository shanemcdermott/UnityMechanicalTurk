using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


using Framework.Collections;
using Framework.Generation;
using Algorithms.City;

namespace Algorithms.Util
{

    //Shane McDermott 2018
    public class ClutterPlacement : CityBlockGenerator
    {
        public int maxItemsPerNode = 10;

        protected override void PopulateUnvisitedNodes(IEnumerable<GridNode> nodes)
        {
            if (pointGenerator)
            {
                List<Vector2> points;
                foreach (GridNode node in nodes)
                {
                    Vector3 offset = node.GetVertex(0).GetPositionXZ();
                    pointGenerator.Generate(node.Dimensions.x, node.Dimensions.z, MinLotSize.x, maxItemsPerNode, out points);
                    foreach (Vector2 p in points)
                    {
                        Vector3 v = new Vector3(p.x, 0f, p.y) + offset;
                        //new Vector3(point.x, 0f, point.y);
                        v.y = terrain.SampleHeight(v + transform.root.position);
                        GameObject go = SpawnBlockObject(node);
                        if (go)
                        {
                            go.transform.localPosition = v;
                        }
                    }
                }

            }
            else
            {
                foreach (GridNode node in nodes)
                {
                    Vector3 v = node.GetPosition();
                    //new Vector3(point.x, 0f, point.y);
                    v.y = terrain.SampleHeight(v + transform.root.position);
                    node.SetPosition(v);
                    SpawnBlockObject(node);
                }
            }
        }
    }
}