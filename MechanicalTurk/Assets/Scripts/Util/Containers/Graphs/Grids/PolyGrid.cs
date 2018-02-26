using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PolyGrid : MonoBehaviour
{
    [SerializeField]
    protected List<Node> faces = new List<Node>();

    [SerializeField]
	protected List<Node> vertices = new List<Node>();

    public void AddFace(Node newFace)
    {
        faces.Add(newFace);
    }

	public void AddVertex(Node newVertex)
	{
		vertices.Add (newVertex);
	}

	
    public List<Node> GetVertices()
    {
        return vertices;
    }

    public List<Node> GetFaces()
    {
        return faces;
    }

    public void OnDrawGizmosSelected()
    {
        DrawVertices();
        DrawFaces();
    }

    public void DrawVertices()
    {
        foreach(Node node in vertices)
        {
            node.DrawConnections();
        }
    }

    public void DrawFaces()
    {
        foreach (Node node in faces)
        {
            node.DrawConnections();
        }
    }
}
