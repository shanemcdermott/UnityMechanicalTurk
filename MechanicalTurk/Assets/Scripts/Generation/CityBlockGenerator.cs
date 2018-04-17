using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CityBlockGenerator : GenerationAlgorithm
{

    public List<GameObject> blockPrefabs = new List<GameObject>();

    [Header("Grid Settings")]
    /*Total dimensions of the city*/
    public Vector3 Dimensions = new Vector3(256f, 0f, 256f);
    /*Minimum size a plot of land can be */
    public Vector3 MinLotSize = new Vector3(10f, 10f, 10f);

    /*Root node of the block*/
    public GridNode districtNode;
    public Terrain terrain;

    private GameObject cityBlock;

    public override bool CanGenerate()
    {
        return districtNode != null && terrain != null;
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
        //throw new NotImplementedException();
    }

    public override void Clean()
    {
        base.Clean();
        if (cityBlock != null)
        {
            DestroyImmediate(cityBlock);
        }
        districtNode = null;
    }

    public override void Generate()
    {
        SubdivideGrid();
        PopulateBlocks();
    }

    public virtual void CreateGrid()
    {
        GridFaceFactory<GridNode> gFac = new GridFaceFactory<GridNode>();
        /*Also used as the center for the root node*/
        Vector3 lotSize = Dimensions * 0.5f;
        districtNode = gFac.GetSquareNode(lotSize, Dimensions);
    }

    public virtual void SubdivideGrid()
    {
        Vector3 lotSize = Dimensions * 0.5f;
        /*Split the grid in half until the desired leaf size is achieved*/
        while (lotSize.x >= MinLotSize.x && lotSize.z >= MinLotSize.z)
        {
            districtNode.Subdivide();
            lotSize *= 0.5f;
        }
    }

    public virtual void PopulateBlocks()
    {
        List<GridNode> leaves;
        districtNode.GetLeaves(out leaves);
        foreach (GridNode child in leaves)
        {
            Vector3 v = child.GetPosition();
            v.y = terrain.SampleHeight(v+transform.root.position);
            child.SetPosition(v);
            SpawnCityBlock(child);
        }
    }

    public virtual void SpawnCityBlock(GridNode parentNode)
    {
        GameObject go = GameObject.Instantiate(ChooseDistrictToSpawn(parentNode));
        go.transform.SetParent(cityBlock.transform);
        go.transform.localPosition = parentNode.GetPosition();
        CityBlockGenerator blockGen = go.GetComponent<CityBlockGenerator>();
        if (blockGen)
        {
            blockGen.districtNode = parentNode;
            blockGen.Dimensions = MinLotSize;
            blockGen.terrain = terrain;
            blockGen.Setup();
            if (blockGen.CanGenerate())
            {
                blockGen.Generate();
            }
        }
        else
        {
            BoxCollider collider = go.GetComponent<BoxCollider>();
            if(collider)
            {
                Vector3 v = new Vector3(-collider.center.x, 0f, -collider.center.z);//new Vector3(collider.size.x * 0.5f,0f, collider.size.z * -0.5f);
                //v.y = 0;
                go.transform.localPosition += v;
                
                // Renderer rend = go.transform.GetComponent<Renderer>();
                // Vector3 v = go.GetComponent<MeshFilter>().sharedMesh.bounds.center;
                //Vector3 v = collider.size * 0.5f;
                // go.transform.localPosition += new Vector3(collider.size.x * 0.5f, 0f, collider.size.z * -0.25f);
                //go.transform.localPosition += new Vector3(collider.bounds.center.x * 0.5f, 0f, collider.bounds.center.z * 0.5f);
                //go.transform.localPosition += new Vector3(1f, 0f, 1f);//new Vector3(collider.center.x, 0f, collider.center.z);
            }
        }
    }

    public virtual GameObject ChooseDistrictToSpawn(Node parentNode)
    {

        GameObject regionToSpawn = blockPrefabs[Random.Range(0, blockPrefabs.Count)];
        return regionToSpawn;
    }
}
