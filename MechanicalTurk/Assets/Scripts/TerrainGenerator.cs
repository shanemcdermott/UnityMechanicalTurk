using UnityEngine;
using System.Collections;

public class TerrainGenerator : MonoBehaviour
{

    public NoiseMap noiseMap;

    public int depth = 3;

    public float scale = 10f;

    public float offsetX = 100f;
    public float offsetY = 100f;

    void Update()
    {
        Generate();
    }


    public void Generate()
    {
        if(noiseMap == null)
        {
            noiseMap = GetComponent<NoiseGenerator>().noiseMap;
        }
        Terrain terrain = GetComponent<Terrain>();
        terrain.terrainData = GenerateTerrainData(terrain.terrainData);
        terrain.Flush();
    }

    TerrainData GenerateTerrainData(TerrainData terrainData)
    {
        terrainData.heightmapResolution = noiseMap.Width + 1;
        terrainData.size = new Vector3(noiseMap.Width, depth, noiseMap.Height);
		terrainData.SetHeightsDelayLOD(0, 0, noiseMap.Values);
        return terrainData;
    }
	/*
    float[,] GenerateHeights()
    {
        float[,] heights = new float[width, height];


        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                heights[x, y] = CalculateHeight(x, y);
            }
        }
        return heights;
    }

    float CalculateHeight(int x, int y)
    {
        float xf = (float)x / width * scale + offsetX;
        float yf = (float)y / height * scale + offsetY;
        return simplex.noise(xf, yf);
    }
	*/
}