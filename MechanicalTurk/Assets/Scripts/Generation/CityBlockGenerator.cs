﻿using System.Collections;
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
        return districtNode != null;
    }

    public override void Setup()
    {
        cityBlock = new GameObject("CityBlock");
        cityBlock.transform.SetParent(transform);
        cityBlock.transform.position = transform.root.position;
        //throw new NotImplementedException();
    }

    public override void Clean()
    {
        base.Clean();
        if (cityBlock != null)
        {
            DestroyImmediate(cityBlock);
        }
    }

    public override void Generate()
    {
        FinishSubdivision();
        PopulateBlocks();
        //throw new NotImplementedException();
    }

    public virtual void FinishSubdivision()
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

    public virtual void SpawnCityBlock(Node parentNode)
    {
        GameObject go = GameObject.Instantiate(ChooseLotToSpawn(parentNode));
        go.transform.SetParent(cityBlock.transform);
        GameNode gn = go.GetComponent<GameNode>();
        if (gn == null)
        {
            gn = go.AddComponent<GameNode>();
        }
        gn.SetNode(parentNode);
        //gn.SpawnBuildings();
    }

    public virtual GameObject ChooseLotToSpawn(Node parentNode)
    {

        GameObject regionToSpawn = blockPrefabs[Random.Range(0, blockPrefabs.Count)];
        return regionToSpawn;
    }
}
