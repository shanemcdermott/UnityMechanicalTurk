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

public struct DelaunayTriangle
{
    public IntTriangle Triangle;
    private List<DelaunayTriangle> Children;

    public DelaunayTriangle(int A, int B, int C)
    {
        Triangle = new IntTriangle(A, B, C);
        Children = new List<DelaunayTriangle>();
    }

    public DelaunayTriangle(IntTriangle Tri)
    {
        this.Triangle = Tri;
        Children = new List<DelaunayTriangle>();
    }

    public void AddChild(DelaunayTriangle dt)
    {
        Children.Add(dt);
    }

    public void RemoveChild(DelaunayTriangle dt)
    {
        Children.Remove(dt);
    }

    public List<DelaunayTriangle> GetTriangles()
    {
        if (Children == null || Children.Count == 0)
            return new List<DelaunayTriangle>(new DelaunayTriangle[] { new DelaunayTriangle(this.Triangle) });
        return Children;
    }

    public IntPoint[] Edges()
    {
        return Triangle.Edges();
    }

    public IntPoint[] AllEdges()
    {
        List<DelaunayTriangle> tris = GetTriangles();
        List<IntPoint> e = new List<IntPoint>();
        foreach(DelaunayTriangle dt in tris)
        {
            
            e.AddRange(dt.Edges());
        }


        return e.ToArray();
    }

    public bool SharesVertexWith(DelaunayTriangle other)
    {
        return Triangle.SharesVertexWith(other.Triangle);
    }

    public bool HasVertex(int a)
    {
        return Triangle.HasVertex(a);
    }

    public bool HasEdge(IntPoint edge)
    {
        return Triangle.HasEdge(edge);
    }

    public bool HasEdge(int a, int b)
    {
        return Triangle.HasEdge(a, b);
    }

    public override bool Equals(object obj)
    {
        if (!(obj is DelaunayTriangle))
            return false;

        DelaunayTriangle dt = (DelaunayTriangle)obj;
        // compare elements here
        return Triangle.Equals(dt.Triangle);

    }
}

public class DelaunayTriangulation : MonoBehaviour
{
    [SerializeField]
	public PolygonCollider2D bounds;
    [SerializeField]
    protected int NumTriangles = 0;
    [SerializeField]
    protected int NumPoints = 0;

    protected List<Vector2> points = new List<Vector2>();

    DelaunayTriangle superTriangle;

    protected List<IntTriangle> triangles = new List<IntTriangle>();

    public void Init()
    {
        
        points = new List<Vector2>(Corners());
        triangles = new List<IntTriangle>();
        superTriangle = new DelaunayTriangle(0, 1, 2);

        triangles.Add(new IntTriangle(0, 1, 2));
        triangles.Add(new IntTriangle(2, 3, 0));
        triangles.Add(new IntTriangle(3, 4, 0));
        NumTriangles = triangles.Count;
        NumPoints = points.Count;
    }

    public Vector2[] Corners()
    {
		return bounds.points;
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

    protected void UpdateDeltris(Vector2 NewPoint)
    {
        List<DelaunayTriangle> badTriangles = new List<DelaunayTriangle>();
        // first find all the triangles that are no longer valid due to the insertion

        foreach (DelaunayTriangle dt in superTriangle.GetTriangles())
        {
            if (IsPointInsideCircumcircle(NewPoint, dt))
                badTriangles.Add(dt);
        }

        //Find the boundary of the polygonal hole
        List<IntPoint> polygon = new List<IntPoint>();
        foreach (DelaunayTriangle dt in badTriangles)
        {
            foreach (IntPoint edge in dt.Edges())
            {
                //if edge is not shared by any other triangles in badTriangles
                //add edge to polygon
                bool bIsShared = false;
                foreach (DelaunayTriangle bdt in badTriangles)
                {
                    if (bdt.Equals(dt)) continue;

                    bIsShared = bdt.HasEdge(edge);
                    if (bIsShared) break;
                }
                if (!bIsShared)
                    polygon.Add(edge);
            }
        }
        //Remove bad triangles from the triangulation
        foreach (DelaunayTriangle dt in badTriangles)
        {
            superTriangle.RemoveChild(dt);
        }

        int newPointIndex = points.Count - 1;
        //Re-triangulate the polygonal hole
        foreach (IntPoint edge in polygon)
        {
            DelaunayTriangle dt = new DelaunayTriangle(edge.x, edge.y, newPointIndex);
            superTriangle.AddChild(dt);
        }

        foreach (DelaunayTriangle dt in superTriangle.GetTriangles())
        {
            if (superTriangle.SharesVertexWith(dt))
                superTriangle.RemoveChild(dt);
        }
    }

    public bool IsPointInsideCircumcircle(Vector2 p, DelaunayTriangle dt)
    {
        return IsPointInsideCircumcircle(p, dt.Triangle);
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
