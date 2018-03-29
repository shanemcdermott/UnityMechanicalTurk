using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinCityGenerator : CityGenerator {

    public override void Generate()
    {
        GenerateRoads();
        GenerateBuildings();
    }

    void GenerateRoads()
    {
        if (polyGrid.NumFaces() == 0)
        {
            Debug.Log("populating perlin road grid");
            GridFactory.PopulateSquareGrid(ref polyGrid);
            foreach (Node node in polyGrid.GetFaces())
            {
                GameObject go = GameObject.Instantiate(GameObject.CreatePrimitive(PrimitiveType.Cube));
                go.transform.SetParent(transform);
                GameNode gn = go.GetComponent<GameNode>();
                gn.SetNode(node);
                gn.SpawnBuildings();
            }
        }
    }

    void GenerateBuildings()
    {

    }

    public override void Setup()
    {
        polyGrid.Dimensions = heightMap.Dimensions;
        polyGrid.FacesPerSide = new Vector2Int(2, 2);
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
