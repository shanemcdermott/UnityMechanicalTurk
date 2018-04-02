using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridFace : Node
{
    protected List<Node> vertices;

    public GridFace()
    {
        position = new Vector3();
        vertices = new List<Node>();
    }

    public GridFace(Vector2 position2)
    {
        position = new Vector3(position2.x, 0, position2.y);
        vertices = new List<Node>();
    }

    public GridFace(Vector3 position)
    {
        this.position = position;
        vertices = new List<Node>();
    }

    public void AddVertices(Node[] vertices)
    {
        this.vertices.AddRange(vertices);
    }

    public int AddVertex(Node vertex)
    {
        vertices.Add(vertex);
        return vertices.Count - 1;
    }

    public int NumVertices()
    {
        return vertices.Count;
    }

    public Node GetVertex(int index)
    {
        return vertices[index];
    }

    public bool HasVertex(Node vertex)
    {
        return vertices.Contains(vertex);
    }

    public Node GetConnectionWithVertex(Node vertex)
    {
        int i = FindConnectionWithVertex(vertex);
        if (i != -1)
        {
            return connections[i];
        }
        return null;
    }

    protected int FindConnectionWithVertex(Node vertex)
    {
        for (int i = 0; i < NumConnections(); i++)
        {
            if (connections[i] != null)
            {
                GridFace face = (GridFace)connections[i];
                if (face != null && face.HasVertex(vertex))
                    return i;
            }
        }

        return -1;
    }

    public List<Vector3> GetVertexPositions()
    {
        List<Vector3> pos = new List<Vector3>();
        foreach (Node node in vertices)
        {
            pos.Add(node.GetPosition());
        }
        return pos;
    }

    public Node[] GetVertices()
    {
        return vertices.ToArray();
    }

    protected void SwapVertices(int i, int j)
    {
        Node ni = vertices[i];
        vertices[i] = vertices[j];
        vertices[j] = ni;
    }

    public override void DrawConnections()
    {
        Vector3 pNorm = position.normalized;

        Color color = Color.cyan;
        foreach (GridFace c in connections)
        {
            if (c != null)
            {
                Debug.DrawLine(position, c.position, color);
            }
        }
    }
}
