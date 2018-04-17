using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityBiomeGenerator : CityGenerator
{
    public Terrain terrain;
    public RoadPainter roadPainter;
    public CityBlockGenerator regionGenerator;

    public bool bShouldDrawFromCenter = true;
    public GridNode gridNode;

    public override void Setup()
    {
        base.Setup();
        Clean();
        regionGenerator.terrain = terrain;
        regionGenerator.Setup();
        roadPainter.Setup();
    }

    public override void Clean()
    {
        base.Clean();
        if (regionGenerator != null)
        {
            regionGenerator.Clean();
        }
        while(transform.childCount > 0)
        {
            Transform child = transform.GetChild(0);
            DestroyImmediate(child.gameObject);
        }
    }

    public override void Generate()
    {
        regionGenerator.Generate();
        gridNode = regionGenerator.districtNode;
        CreateRoadsFromGrid();
    }
    
    public virtual void CreateRoadsFromGrid()
    {

        Dictionary<Vector2Int, bool> connectionPoints = new Dictionary<Vector2Int, bool>();
        //draw connections between face verts
        DrawEdgeRoads(ref connectionPoints);

        if (bShouldDrawFromCenter)
        {
            DrawRoadsFromCenters(ref connectionPoints);
        }

        roadPainter.DrawRoads(ref connectionPoints);
        roadPainter.ApplyAlphaBlend();
    }

    public virtual void DrawEdgeRoads(ref Dictionary<Vector2Int, bool> connectionPoints)
    {
        
        List<Node> vertices = gridNode.GetChildVertices();
        //draw connections between verts

        foreach (Node node in vertices)
        {
             node.GetConnectionLines(ref connectionPoints);
        }

        
    }

    public virtual void DrawRoadsFromCenters(ref Dictionary<Vector2Int, bool> connectionPoints)
    {
        /*
        int j = 0;
        foreach(GridNode child in gridNode.Children)
        {
            int a = 3;
            int b = 2;
            if(child.IsLeaf())
            {
                Vector3 midPoint = MathOps.Midpoint(child.GetVertex(a).GetPosition(), child.GetVertex(b).GetPosition());
                child.GetConnectionLine(ref connectionPoints, child.GetPosition(), midPoint);
            }
        }
        */
        List<GridNode> faces;
        gridNode.GetLeaves(out faces);

        for (int i = 0; i < faces.Count; i++)
        {
            int a = 0;
            int b = 1;
            if (i % 4 < 2)
            {
                a = 3;
                b = 2;
            }

            GetRoadsFromLeaf(ref connectionPoints, faces[i], a, b);
            //int a = (i % 2 == 0)? 3 : 0;
            //int b = (i % 3 == 0)? 1 : 2; 


            //Vector3 midPoint = MathOps.Midpoint(faces[i].GetVertex(a).GetPosition(), faces[i].GetVertex(b).GetPosition());
            //faces[i].GetConnectionLine(ref connectionPoints, faces[i].GetPosition(), midPoint);
        }
    }

    protected void GetRoadsFromLeaf(ref Dictionary<Vector2Int, bool> connectionPoints, GridNode leaf, int a, int b)
    {
        Vector3 midPoint = MathOps.Midpoint(leaf.GetVertex(a).GetPosition(), leaf.GetVertex(b).GetPosition());
        leaf.GetConnectionLine(ref connectionPoints, leaf.GetPosition(), midPoint);
    }

}
