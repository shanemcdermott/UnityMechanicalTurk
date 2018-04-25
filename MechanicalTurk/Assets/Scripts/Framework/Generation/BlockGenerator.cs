using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public struct ObjectSpawnParams
{
    public GameObject[] prefabOptions;
    public float maxSteepness;
    public Vector2 minDimensions;

    public GameObject randomObject
    {
        get
        {
            return prefabOptions[Random.Range(0, prefabOptions.Length)];
        }
    }
}

public class BlockGenerator : GenerationAlgorithm
{
    public List<GameObject> blockPrefabs = new List<GameObject>();
    public PointGenerator pointGenerator;

    [Header("Grid Settings")]
    /*Total dimensions of the city*/
    public Vector3 Dimensions = new Vector3(256f, 0f, 256f);
    /*Minimum size a plot of land can be */
    public Vector3 MinLotSize = new Vector3(10f, 10f, 10f);

    /*Root node of the block*/
    public GridNode districtNode;
    public Terrain terrain;

    public RoadDesigner roadDesigner;
    protected GameObject cityBlock; 

    public override void Clean()
    {
        base.Clean();
        if (cityBlock != null)
        {
            DestroyImmediate(cityBlock);
        }
        districtNode = null;
    }

    public override void Setup()
    {
        cityBlock = new GameObject("CityBlock");
        cityBlock.transform.SetParent(transform);
        cityBlock.transform.position = transform.root.position;
        if (districtNode == null)
        {
            CreateGrid();
        }
        if (roadDesigner == null)
        {
            roadDesigner = GetComponent<RoadDesigner>();
        }
    }

    public virtual void CreateGrid()
    {
        GridFaceFactory<GridNode> gFac = new GridFaceFactory<GridNode>();
        /*Also used as the center for the root node*/
        Vector3 lotSize = Dimensions * 0.5f;
        districtNode = gFac.GetSquareNode(lotSize, Dimensions);
    }

    public override bool CanGenerate()
    {
        return districtNode != null && terrain != null;
    }

    public override void Generate()
    {
        PopulateBlocks();
    }

    public virtual void PopulateBlocks()
    {
        List<GridNode> leaves;
        districtNode.GetLeaves(out leaves);
        List<GridNode> unvisited = new List<GridNode>(leaves);
        Vector2 worldOffset = districtNode.GetVertex(0).GetPositionXZ();
        if (pointGenerator)
        {
            List<Vector2> points;
            pointGenerator.Generate(districtNode.Dimensions.x, districtNode.Dimensions.z, MinLotSize.x, 1000, out points);
            foreach (Vector2 p in points)
            {
                Vector2 point = p + worldOffset;
                GridNode childNode;
                if (districtNode.GetChildContainingPoint(point, out childNode))
                {
                    if (unvisited.Remove(childNode))
                    {
                        Vector3 v = childNode.GetPosition();
                        //new Vector3(point.x, 0f, point.y);
                        v.y = terrain.SampleHeight(v + transform.root.position);
                        childNode.SetPosition(v);
                        SpawnBlockObject(childNode);
                    }
                }
            }
            PopulateUnvisitedNodes(unvisited);
        }
        else
        {
            foreach (GridNode node in leaves)
            {
                Vector3 v = node.GetPosition();
                //new Vector3(point.x, 0f, point.y);
                v.y = terrain.SampleHeight(v + transform.root.position);
                node.SetPosition(v);
                SpawnBlockObject(node);
            }
        }
    }

    public virtual GameObject SpawnBlockObject(GridNode parentNode)
    {
        GameObject go = GameObject.Instantiate(ChooseDistrictToSpawn(parentNode));
        go.transform.SetParent(cityBlock.transform);
        go.transform.localPosition = parentNode.GetPosition();
        BlockGenerator blockGen = go.GetComponent<BlockGenerator>();
        if (blockGen)
        {
            blockGen.districtNode = parentNode;
            blockGen.Dimensions = parentNode.Dimensions;
            blockGen.terrain = terrain;
            blockGen.Setup();
            if (blockGen.CanGenerate())
            {
                blockGen.Generate();
            }
        }
        return go;
    }

    /*By default, does nothing*/
    protected virtual void PopulateUnvisitedNodes(IEnumerable<GridNode> nodes)
    {

    }

    public virtual GameObject ChooseDistrictToSpawn(Node parentNode)
    {

        GameObject regionToSpawn = blockPrefabs[Random.Range(0, blockPrefabs.Count)];
        return regionToSpawn;
    }

    public virtual void GetRoads(ref Dictionary<Vector2Int, bool> roads)
    {
        if (roadDesigner)
        {
            roadDesigner.GetRoads(districtNode, ref roads);
        }
    }

}
