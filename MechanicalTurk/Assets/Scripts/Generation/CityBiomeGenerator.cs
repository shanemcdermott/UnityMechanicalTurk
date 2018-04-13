using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityBiomeGenerator : CityGenerator
{
    public Terrain terrain;
    public RoadPainter roadPainter;

    [Header("Grid Settings")]
    /*Total dimensions of the city*/
    public Vector3 Dimensions = new Vector3(256f,0f,256f);
    /*Minimum size a plot of land can be */
    public Vector3 MinLotSize = new Vector3(10f, 10f, 10f);

    //Level of detail prefabs. Each should have a GameNode Component;
    public GameObject[] DistrictPrefabs = new GameObject[3];
    public float chanceToPersist = 0.66f;
    public int[] spawnWeights = new int[10] { 1, 0, 0, 0,
                                                1, 1, 1,
                                                2, 2, 2 };

    public bool bShouldDrawFromCenter = true;
    public GridNode gridNode;

    public override void Setup()
    {
        base.Setup();
        roadPainter.Setup();
    }

    public override void Clean()
    {
        base.Clean();

    }

    public override void Generate()
    {
        CreateGrid();
        SpawnRegions();
        CreateRoadsFromGrid();
    }

    public virtual void CreateGrid()
    {
        GridFaceFactory<GridNode> gFac = new GridFaceFactory<GridNode>();
        /*Also used as the center for the root node*/
        Vector3 lotSize = Dimensions * 0.5f;
        gridNode = gFac.GetSquareNode(lotSize, Dimensions);

        /*Split the grid in half until the desired leaf size is achieved*/
        while(lotSize.x >= MinLotSize.x && lotSize.z >= MinLotSize.z)
        {
            gridNode.Subdivide();
            lotSize *= 0.5f;
        }
    }

    public virtual void SpawnRegions()
    {
        List<GridNode> leaves;
        gridNode.GetLeaves(out leaves);
        foreach (GridNode child in leaves)
        {
            SpawnRegion(child);
        }
        
    }

    public virtual void SpawnRegion(GridNode parentNode)
    {
        GameObject go = GameObject.Instantiate(ChooseRegionToSpawn(parentNode));
        go.transform.SetParent(transform);
        GameNode gn = go.GetComponent<GameNode>();
        gn.SetNode(parentNode);
        CityBlockGenerator blockGen = gn.GetComponent<CityBlockGenerator>();
        if(blockGen)
        {
            blockGen.districtNode = parentNode;
            blockGen.Dimensions = MinLotSize;
            blockGen.terrain = terrain;
            blockGen.Setup();
            if(blockGen.CanGenerate())
            {
                blockGen.Generate();
            }
        }
    }

    public virtual GameObject ChooseRegionToSpawn(Node parentNode)
    {
        int i = Random.Range(0, spawnWeights.Length);
        GameObject regionToSpawn = DistrictPrefabs[spawnWeights[i]];

        if (Random.value > chanceToPersist)
        {
            spawnWeights[i] = Random.Range(0, DistrictPrefabs.Length);
        }
        return regionToSpawn;
    }

    public virtual void CreateRoadsFromGrid()
    {
        List<Node> vertices = gridNode.GetChildVertices();

        List<GridNode> faces;
        gridNode.GetLeaves(out faces);

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
                int a = Random.value > 0.5f ? 0 : 3;
                int b = Random.value > 0.5f ? 1 : 2; 

                Vector3 midPoint = MathOps.Midpoint(faces[i].GetVertex(a).GetPosition(), faces[i].GetVertex(b).GetPosition());
                faces[i].GetConnectionLine(ref connectionPoints, faces[i].GetPosition(), midPoint);
            }
        }


        roadPainter.DrawRoads(ref connectionPoints);
        roadPainter.ApplyAlphaBlend();
    }
}
