using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PolyGrid : MonoBehaviour
{
    //Spawn Parameters
    public Vector2 Dimensions = new Vector2Int(256,256);
    public Vector2Int FacesPerSide = new Vector2Int(2,2);


    [SerializeField]
    protected List<GridFace> faces = new List<GridFace>();

    [SerializeField]
	protected List<Node> vertices = new List<Node>();

    public int NumFaces()
    {
        return faces.Count;
    }

    public void AddFace(GridFace newFace)
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

    public List<GridFace> GetFaces()
    {
        return faces;
    }

    public void FlipAxes()
    {
        foreach(Node n in vertices)
        {
            n.SetPosition(MathOps.FlipYZ(n.GetPosition()));
        }
        foreach (GridFace n in faces)
        {
            n.SetPosition(MathOps.FlipYZ(n.GetPosition()));
        }
    }

    public void OnDrawGizmos()
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
    
    public void Clean(){
        faces = new List<GridFace>();
        vertices = new List<Node>();
    }
}
