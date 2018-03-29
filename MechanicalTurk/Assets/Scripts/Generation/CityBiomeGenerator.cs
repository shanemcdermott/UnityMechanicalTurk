using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityBiomeGenerator : CityGenerator
{
    //Level of detail prefabs. Each should have a GameNode Component;
    public GameObject[] LOD_0_Prefabs = new GameObject[3];

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
