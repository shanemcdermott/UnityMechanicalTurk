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
    protected List<Node> vertices
    {
        get
        {
            return new List<Node>(verts.Values);
        }
        set
        {
            foreach(Node n in value)
            {
                FindOrAdd(n);
            }
        }
    }

    private Dictionary<Vector2Int, Node> verts = new Dictionary<Vector2Int, Node>();

    /// <summary>
    /// Adds all Leaf nodes of a GridNode to the polygrid
    /// </summary>
    /// <param name="rootNode">
    /// The root node of the grid to add
    /// </param>
    public void AddGridLeaves(ref GridNode rootNode)
    {
        List<GridNode> leaves;
        rootNode.GetLeaves(out leaves);
        foreach (GridNode leaf in leaves)
        {
            faces.Add(leaf);
            foreach(Node vertex in leaf.GetVertices())
            {
                vertices.Add(vertex);
            }
        }
    }

    public int NumFaces()
    {
        return faces.Count;
    }

    public int NumVertices()
    {
        return vertices.Count;
    }

    public void AddFace(GridFace newFace)
    {
        faces.Add(newFace);
    }

    public void FindOrAdd(Node newVertex)
    {
        if(!verts.ContainsValue(newVertex))
        {
            Vector2 v = newVertex.GetPosition();
            verts.Add(new Vector2Int((int)v.x,(int)v.y), newVertex);
        }
    }

    public void UpdateNodeAt(Vector2Int position, Node node)
    {
        if(!verts.ContainsKey(position))
        {
            verts.Add(position, node);
        }
        verts[position] = node;
    }

    public Node GetNodeAt(Vector2Int position)
    {
        if (!verts.ContainsKey(position))
            return null;

        return verts[position];
    }

	public void AddVertex(Node newVertex)
	{
		vertices.Add (newVertex);
	}

    public void AddConnection(int index, Node connection)
    {
        vertices[index].AddConnection(connection);
    }

    public int NumVertexConnections(int index)
    {
        return vertices[index].NumConnections();
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
        verts = new Dictionary<Vector2Int, Node>();
    }
}
