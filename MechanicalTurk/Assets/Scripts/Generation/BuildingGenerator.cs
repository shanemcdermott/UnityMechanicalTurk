using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingGenerator : GenerationAlgorithm {
    [System.Serializable]
    public struct BuildingType
    {
        public string name;
        public float height;
        public GameObject building;
    }

    public BuildingType[] buildingTypes;

    public NoiseMap heightMap;

    public int[,] buildingMap;

    public override bool CanGenerate()
    {
        return buildingTypes != null &&
            buildingTypes.Length > 0 &&
            heightMap != null;
    }

    public override void Setup()
    {
        if (heightMap == null)
        {
            heightMap = GetComponent<NoiseMap>();
        }
    }

    public override void Generate()
    {
        buildingMap = new int[heightMap.Width, heightMap.Height];
        for (int x = 0; x < heightMap.Width; x++)
        {
            for (int y = 0; y < heightMap.Height; y++)
            {
                buildingMap[x, y] = FindBestTerrain(heightMap[x, y]);
            }
        }
    }

    public GameObject[] GetBuildingMap()
    {
        GameObject[] flatBuildingMap = new GameObject[heightMap.Width * heightMap.Height];
        for (int x = 0; x < heightMap.Width; x++)
        {
            for (int y = 0; y < heightMap.Height; y++)
            {
                Debug.Log("types at " + buildingMap[x, y]);
                
                Debug.Log(" type " + buildingTypes[buildingMap[x, y]]);
                flatBuildingMap[heightMap.ToIndex(x, y)] = buildingTypes[buildingMap[x, y]].building;
            }
        }

        return flatBuildingMap;
    }

    public int FindBestTerrain(float heightValue)
    {
        for (int i = 0; i < buildingTypes.Length; i++)
        {
            if (heightValue <= buildingTypes[i].height)
            {
                Debug.Log("best: " + i);
                return i;
            }
                
        }

        return 0;
    }
}
