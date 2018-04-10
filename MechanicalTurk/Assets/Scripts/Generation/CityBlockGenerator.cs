using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CityBlockGenerator : GenerationAlgorithm
{

    public List<GameObject> LotPrefabs = new List<GameObject>();

    [Header("Grid Settings")]
    /*Total dimensions of the city*/
    public Vector3 Dimensions = new Vector3(256f, 0f, 256f);
    /*Minimum size a plot of land can be */
    public Vector3 MinLotSize = new Vector3(10f, 10f, 10f);

    /*Root node of the block*/
    public GridNode blockNode;


    public override bool CanGenerate()
    {
        return blockNode != null;
    }

    public override void Setup()
    {
        //throw new NotImplementedException();
    }

    public override void Generate()
    {
        CreateGrid();
        PopulateBlocks();
        //throw new NotImplementedException();
    }

    public virtual void CreateGrid()
    {
        /*Also used as the center for the root node*/
        Vector3 lotSize = Dimensions * 0.5f;

        /*Split the grid in half until the desired leaf size is achieved*/
        while (lotSize.x >= MinLotSize.x && lotSize.z >= MinLotSize.z)
        {
            blockNode.Subdivide();
            lotSize *= 0.5f;
        }
    }

    public virtual void PopulateBlocks()
    {
        List<GridNode> leaves;
        blockNode.GetLeaves(out leaves);
        foreach (GridNode child in leaves)
        {
            SpawnRegion(child);
        }
    }

    public virtual void SpawnRegion(Node parentNode)
    {
        GameObject go = GameObject.Instantiate(ChooseLotToSpawn(parentNode));
        go.transform.SetParent(transform);
        GameNode gn = go.GetComponent<GameNode>();
        gn.SetNode(parentNode);
        gn.SpawnBuildings();
    }

    public virtual GameObject ChooseLotToSpawn(Node parentNode)
    {

        GameObject regionToSpawn = LotPrefabs[Random.Range(0, LotPrefabs.Count)];
        return regionToSpawn;
    }
}
