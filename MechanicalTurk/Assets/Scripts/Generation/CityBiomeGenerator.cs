using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityBiomeGenerator : CityGenerator
{
    public Terrain terrain;
    public RoadPainter roadPainter;
    //Level of detail prefabs. Each should have a GameNode Component;
    public GameObject[] LOD_0_Prefabs = new GameObject[3];
    public float chanceToPersist = 0.66f;
    public int[] spawnWeights = new int[10] { 1, 0, 0, 0,
                                                1, 1, 1,
                                                2, 2, 2 };

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
        SpawnRegions();
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

    public virtual void SpawnRegions()
    {
        foreach (Node node in polyGrid.GetFaces())
        {
            SpawnRegion(node);
        }
        
    }

    public virtual void SpawnRegion(Node parentNode)
    {
        GameObject go = GameObject.Instantiate(ChooseRegionToSpawn(parentNode));
        go.transform.SetParent(transform);
        GameNode gn = go.GetComponent<GameNode>();
        gn.SetNode(parentNode);
        gn.SetTerrain(ref terrain);
        gn.SpawnBuildings();
    }

    public virtual GameObject ChooseRegionToSpawn(Node parentNode)
    {
        int i = Random.Range(0, spawnWeights.Length);
        GameObject regionToSpawn = LOD_0_Prefabs[spawnWeights[i]];

        if (Random.value > chanceToPersist)
        {
            spawnWeights[i] = Random.Range(0, LOD_0_Prefabs.Length);
        }
        return regionToSpawn;
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

        if (bShouldDrawFromCenter)
        {
            for (int i = 0; i < faces.Count; i++)
            {
                int j = Random.value > 0.5f ? 1 : 2;

                Vector3 midPoint = MathOps.Midpoint(faces[i].GetVertex(0).GetPosition(), faces[i].GetVertex(j).GetPosition());
                faces[i].GetConnectionLine(ref connectionPoints, faces[i].GetPosition(), midPoint);
                //faces[i].GetConnectionLine(ref connectionPoints, faces[i].GetVertex(0), faces[i].GetVertex(2));
                //faces[i].GetConnectionLine(ref connectionPoints, faces[i].GetVertex(2), faces[i].GetVertex(3));
                //faces[i].GetConnectionLine(ref connectionPoints, faces[i].GetVertex(1), faces[i].GetVertex(3));
            }
        }


        roadPainter.DrawRoads(ref connectionPoints);
        roadPainter.ApplyAlphaBlend();
    }
}
