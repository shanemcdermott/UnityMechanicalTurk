using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityBiomeGenerator : CityGenerator
{
    public Terrain terrain;
    public RoadPainter roadPainter;
    //Level of detail prefabs. Each should have a GameNode Component;
    public GameObject[] LOD_0_Prefabs = new GameObject[3];
    public bool bShouldDrawFromCenter = true;

    public override void Setup()
    {
        base.Setup();
        roadPainter.Setup();
    }

    public override void Generate()
    {
        if (polyGrid.NumFaces() != 0)
        {
            polyGrid.Clean();
        }
        
        Debug.Log("populating grid");
        GridFactory.PopulateSquareGrid(ref polyGrid);
        //AssignRegionTypes();
        SpawnBuildings();
        CreateRoadsFromGrid();
    }

    public virtual void AssignRegionTypes()
    {
        List<Node> roadVerts = polyGrid.GetVertices();
        foreach(Node n in roadVerts)
        {
            List<IConnection<Node>> nodeLinks;
            n.GetConnections(out nodeLinks);
            foreach(IConnection<Node> nodeLink in nodeLinks)
            {
                roadPainter.DrawLine(nodeLink.GetFromNode(), nodeLink.GetToNode());
            }
        }

        roadPainter.ApplyAlphaBlend();
    }

    public virtual void CreateRoadsFromGrid()
    {
        List<Node> vertices = polyGrid.GetVertices();
        List<GridFace> faces = polyGrid.GetFaces();

        Dictionary<Vector2Int, bool> connectionPoints = new Dictionary<Vector2Int, bool>();
        //draw connections between verts
        foreach (Node node in vertices)
        {
           node.GetConnectionLines(ref connectionPoints);
        }

        //draw connections between face verts
        
        for (int i = 0; i < faces.Count &&bShouldDrawFromCenter; i++)
        {
            faces[i].GetConnectionLine(ref connectionPoints, faces[i].GetPosition(), faces[i].GetVertex(1).GetPosition());
            //faces[i].GetConnectionLine(ref connectionPoints, faces[i].GetVertex(0), faces[i].GetVertex(2));
            //faces[i].GetConnectionLine(ref connectionPoints, faces[i].GetVertex(2), faces[i].GetVertex(3));
            //faces[i].GetConnectionLine(ref connectionPoints, faces[i].GetVertex(1), faces[i].GetVertex(3));
        }
        

        roadPainter.DrawRoads(ref connectionPoints);
        roadPainter.ApplyAlphaBlend();
    }

    public virtual void SpawnBuildings()
    {
        foreach (Node node in polyGrid.GetFaces())
        {
            SpawnBuilding(node);
        }
        
    }

    public virtual void SpawnBuilding(Node parentNode)
    {
        GameObject go = GameObject.Instantiate(LOD_0_Prefabs[Random.Range(0, LOD_0_Prefabs.Length)]);
        go.transform.SetParent(transform);
        GameNode gn = go.GetComponent<GameNode>();
        gn.SetNode(parentNode);
        gn.SetTerrain(ref terrain);
        gn.SpawnBuildings();
    }
}
