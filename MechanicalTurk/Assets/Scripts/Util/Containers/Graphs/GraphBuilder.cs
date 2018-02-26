using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphBuilder : MonoBehaviour
{
    public DelaunayTriangulation delTri;
    public bool drawTriangles = false;

    public PointGenerator pointGenerator;
    public bool drawPoints = true;

    public void Init()
    {
        pointGenerator.Init(pointGenerator.Corners());
        InitializeGraph();
    }

    public void InitializeGraph()
    {
        delTri.Init();
    }

    public void NextPoint()
    {
        Vector2 v2 = pointGenerator.NextPoint();
        if (v2.Equals(Vector2.positiveInfinity)) return;
        delTri.AddPoint(v2);
    }
  
    public void FillPoints()
    {
        for (Vector2 v2 = pointGenerator.NextPoint(); !(v2.Equals(Vector2.positiveInfinity)); v2 = pointGenerator.NextPoint())
        {
            delTri.AddPoint(v2);
        }
    }
}
