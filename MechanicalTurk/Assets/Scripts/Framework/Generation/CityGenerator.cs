using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityGenerator : GenerationAlgorithm
{
    public NoiseMap heightMap;

    public PolyGrid polyGrid;
    //Level of detail prefabs. Each should have a GameNode Component;
    public GameObject[] LOD_0_Prefabs = new GameObject[3];


    public override bool CanGenerate()
    {
        return heightMap != null;
    }

    public override void Setup()
    {
        if (polyGrid == null)
        {
            polyGrid = gameObject.GetComponent<PolyGrid>();
            if (polyGrid == null)
            {
                polyGrid = gameObject.AddComponent<PolyGrid>();
                polyGrid.Dimensions = heightMap.Dimensions;
                polyGrid.FacesPerSide = new Vector2Int(2, 2);
            }
        }
    }

    public override void Generate()
    {
        if (polyGrid.NumFaces() == 0)
        {
            Debug.Log("populating grid");
            GridFactory.PopulateSquareGrid(ref polyGrid);
            foreach (Node node in polyGrid.GetFaces())
            {
                GameObject go = GameObject.Instantiate(LOD_0_Prefabs[Random.Range(0, LOD_0_Prefabs.Length)]);
                go.transform.SetParent(transform);
                GameNode gn = go.GetComponent<GameNode>();
                gn.SetNode(node);
                gn.SpawnBuildings();
            }
        }
    }
}
