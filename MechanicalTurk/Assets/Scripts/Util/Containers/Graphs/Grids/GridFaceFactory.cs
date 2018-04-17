using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridFaceFactory<T> where T : GridFace, new()
{

	public T GetNewGridFace(Vector3 pos, Node[] verts)
	{
		T res = new T();
		res.SetPosition(pos);
		res.AddVertices(verts);
		return res;
	}

    public T GetSquareNode(Vector3 center, Vector3 Dimensions)
    {
        Vector3 half = Dimensions * 0.5f;
        Node[] verts = new Node[4]
        {
            new Node(center - half),
            new Node(new Vector3(center.x+half.x, center.y, center.z-half.z)),
            new Node(new Vector3(center.x-half.x, center.y, center.z+half.z)),
            new Node(center + half)
        };

        T node = GetNewGridFace(center, verts);
        GridNode gridNode = node as GridNode;
        if(gridNode != null)
        {
            gridNode.ConnectVertices();
        }

        return node;
    }
}
