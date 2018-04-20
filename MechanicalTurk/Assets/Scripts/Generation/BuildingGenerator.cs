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

    public GameObject GetBuilding(Vector2 midpoint, Vector2 faceSize)
    {
        if (midpoint.x >= heightMap.Width || midpoint.y >= heightMap.Height || midpoint.x < 0 || midpoint.y < 0) return null;// buildingTypes[0].building;

        float heightValue = heightMap.Values[(int)midpoint.x, (int)midpoint.y];
        int buildingIndex = FindBestTerrain(heightValue);

        GameObject buildingToSpawn = buildingTypes[buildingIndex].building;
        CityBlockGenerator blockGenerator = buildingToSpawn.GetComponent<CityBlockGenerator>();

        if(blockGenerator != null)
        {
            if(blockGenerator.MinLotSize.x < faceSize.x && blockGenerator.MinLotSize.y < faceSize.y)
            {
                return blockGenerator.gameObject;
            }
        }
        else
        {
            BoxCollider collider = buildingToSpawn.GetComponent<BoxCollider>();
            if(collider != null)
            {
                if (collider.size.x < faceSize.x && collider.size.z < faceSize.y)
                {
                    return buildingToSpawn;
                }
            }
        }

        return null;
    }

    public int FindBestTerrain(float heightValue)
    {
        for (int i = 0; i < buildingTypes.Length; i++)
        {
            if (heightValue <= buildingTypes[i].height)
            {
                return i;
            }
                
        }

        return 0;
    }

    public override void Generate()
    {
        throw new System.NotImplementedException();
    }
}
