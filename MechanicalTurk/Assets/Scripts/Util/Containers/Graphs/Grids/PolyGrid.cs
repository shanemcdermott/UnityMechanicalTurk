using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PolyGrid : MonoBehaviour
{
    [SerializeField]
    protected List<Node> faces = new List<Node>();

    [SerializeField]
	protected List<Node> vertices = new List<Node>();


	public void AddVertex(Node newVertex)
	{
		vertices.Add (newVertex);
	}

	public void Triangulate()
	{
		
	}
	
    public List<Node> GetVertices()
    {
        return vertices;
    }

    public List<Node> GetFaces()
    {
        return faces;
    }
}
