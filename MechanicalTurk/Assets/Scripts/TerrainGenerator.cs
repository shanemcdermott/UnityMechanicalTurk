using UnityEngine;
using System.Collections;

public class TerrainGenerator : MonoBehaviour
{

    SimplexNoise simplex;

    public int depth = 3;
    public int width = 256;
    public int height = 256;

    public float scale = 10f;

    public float offsetX = 100f;
    public float offsetY = 100f;

    public int seed = 0;

    void Start()
    {
        simplex = GetComponent<SimplexNoise>();
        simplex.init(seed);
        offsetX = Random.Range(0f, 9999f);
        offsetY = Random.Range(0f, 9999f);
    }

    void Update()
    {
        Terrain terrain = GetComponent<Terrain>();
        terrain.terrainData = GenerateTerrain(terrain.terrainData);
        terrain.Flush();
    }

    TerrainData GenerateTerrain(TerrainData terrainData)
    {
        terrainData.heightmapResolution = width + 1;
        terrainData.size = new Vector3(width, depth, height);
        terrainData.SetHeightsDelayLOD(0, 0, GenerateHeights());
        return terrainData;
    }

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
}