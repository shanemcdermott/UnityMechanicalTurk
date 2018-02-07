using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct CircumCirc
{
    public Vector2 Center;
    public float Radius;

    public CircumCirc(Vector2 C, float r)
    {
        this.Center = C;
        this.Radius = r;
    }

    public override string ToString()
    {
        return "C: " + Center.ToString() + "R: " + Radius;
    }
}

public struct IntTriangle
{
    public int A;
    public int B;
    public int C;

    public Color color;

    public IntTriangle(int A, int B, int C)
    {
        this.A = A;
        this.B = B;
        this.C = C;
        color = new Color(Random.value, Random.value, Random.value);
    }

    public IntPoint[] Edges()
    {
        return new IntPoint[] { new IntPoint(A, B), new IntPoint(B, C), new IntPoint(C, A) };
    }

    public bool SharesVertexWith(IntTriangle other)
    {
        return HasVertex(other.A) || HasVertex(other.B) || HasVertex(other.C);
    }

    public bool HasVertex(int a)
    {
        return a == A || a == B || a == C;
    }

    public bool HasEdge(IntPoint edge)
    {
        return HasEdge(edge.x, edge.y);
    }

    public bool HasEdge(int a, int b)
    {
        return (HasVertex(a) && HasVertex(b));
    }

    public override bool Equals(object obj)
    {
        if (!(obj is IntTriangle))
            return false;

       IntTriangle dt = (IntTriangle)obj;
        // compare elements here
        return dt.HasVertex(A) && dt.HasVertex(B) && dt.HasVertex(C);

    }

    public override string ToString()
    {
        return "(" + A + "," + B + "," + C + ")";
    }
}

public struct FTriangle
{
	public Vector2 A;
	public Vector2 B;
	public Vector2 C;
	
	public FTriangle(Vector2 A, Vector2 B, Vector2 C)
	{
		this.A = A;
		this.B = B;
		this.C = C;
	}
}

public struct PolyNode
{
	public Vector2 Center;
	public Vector2[] Points;
	
	public PolyNode(Vector2 Center, Vector2[] Points)
	{
		this.Center = Center;
		this.Points = Points;
	}
}

public class DelaunayTriangulation : MonoBehaviour
{
    [SerializeField]
    protected Rect bounds = new Rect(0, 0, 100, 100);
    [SerializeField]
    protected int NumTriangles = 0;
    [SerializeField]
    protected int NumPoints = 0;

    protected List<Vector2> points = new List<Vector2>();

    protected List<IntTriangle> triangles = new List<IntTriangle>();

    public void Init()
    {
        
        points = new List<Vector2>(Corners());
        triangles = new List<IntTriangle>();

        triangles.Add(new IntTriangle(0, 1, 2));
        triangles.Add(new IntTriangle(2, 3, 0));
    }

    public Vector2[] Corners()
    {
        Vector2 bottomLeft = new Vector2(bounds.xMin, bounds.yMin);
        Vector2 topLeft = new Vector2(bounds.xMin, bounds.yMax);
        Vector2 bottomRight = new Vector2(bounds.xMax, bounds.yMin);
        Vector2 topRight = new Vector2(bounds.xMax, bounds.yMax);
        return new Vector2[] {topLeft, bottomLeft,bottomRight, topRight};
    }


    protected void UpdateTriangles(Vector2 NewPoint)
    {
       
        List<IntTriangle> badTriangles = new List<IntTriangle>();
        // first find all the triangles that are no longer valid due to the insertion

        for(int i =0; i < triangles.Count; i++)
        {
            if (IsPointInsideCircumcircle(NewPoint, triangles[i]))
            {
                Debug.Log(NewPoint + "Is inside " + triangles[i].ToString());
                badTriangles.Add(triangles[i]);
            }
        }

        //Find the boundary of the polygonal hole
        List<IntPoint> polygon = new List<IntPoint>();
        foreach (IntTriangle i in badTriangles)
        {
            foreach (IntPoint edge in i.Edges())
            {
                //if edge is not shared by any other triangles in badTriangles
                //add edge to polygon
                bool bIsShared = false;
                foreach (IntTriangle j in badTriangles)
                {
                    if (j.Equals(i)) continue;

                    bIsShared = j.HasEdge(edge);
                    if (bIsShared) break;
                }
                if (!bIsShared)
                    polygon.Add(edge);
            }
        }
        
        int newPointIndex = points.Count - 1;

        IntTriangle[] newTris = new IntTriangle[polygon.Count];
        //Re-triangulate the polygonal hole
        for(int i = 0; i < polygon.Count; i++)
        {
            IntPoint edge = polygon[i];
            IntTriangle dt = new IntTriangle(edge.x, edge.y, newPointIndex);
            newTris[i] = dt;
        }

       
        //Remove bad triangles from the triangulation
        foreach (IntTriangle i in badTriangles)
        {
            triangles.Remove(i);
        }

        foreach(IntTriangle it in newTris)
        {
            triangles.Add(it);
        }


        /**
         *      
      for each triangle in triangulation // done inserting points, now clean up
         if triangle contains a vertex from original super-triangle
            remove triangle from triangulation
        */
    }

    public void AddPoint(Vector2 NewPoint)
    {
        points.Add(NewPoint);
        UpdateTriangles(NewPoint);
        NumTriangles = triangles.Count;
        NumPoints = points.Count;
        /**
         *      
      for each triangle in triangulation // done inserting points, now clean up
         if triangle contains a vertex from original super-triangle
            remove triangle from triangulation
        */
    }

    public bool IsPointInsideCircumcircle(Vector2 p, IntTriangle dt)
    {
        return MathOps.IsPointInCircumcircle(p, points[dt.A], points[dt.B], points[dt.C]);
    }

    public Vector2[] GetPoints()
    {
        if(points != null)
            return points.ToArray();

        return Corners();
    }

    public IntPoint[] GetEdges()
    {
        List<IntPoint> edges = new List<IntPoint>();
        foreach(IntTriangle t in triangles)
        {
            edges.AddRange(new List<IntPoint>(t.Edges()));
        }

        return edges.ToArray();
    }

    public IntTriangle[] GetTriangles()
    {
        return triangles.ToArray();
    }

	public FTriangle GetTriangle(int index)
	{
		return new FTriangle(points[triangles[index].A], points[triangles[index].B], points[triangles[index].C]);
	}
	
	public PolyNode[] ToVoronoi()
	{
		
		return new PolyNode[]{};
	}
	
    public void OnDrawGizmosSelected()
    {
        foreach (IntTriangle t in triangles)
        {

            Vector2 A = points[t.A];
            Vector2 B = points[t.B];
            Vector2 C = points[t.C];

            Gizmos.color = t.color;
            Gizmos.DrawLine(A, B);
            Gizmos.DrawLine(A, C);
            Gizmos.DrawLine(B, C);           
        }
    }

    public Color GetPointColor(Vector2 P)
    {
        foreach(IntTriangle t in triangles)
        {
            if(IsPointInsideCircumcircle(P, t))
            {
                return t.color;
            }
        }
        return Color.white;
    }
}
