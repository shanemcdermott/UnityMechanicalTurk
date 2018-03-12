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

    public Texture2D terrainTexture;
    public int seed = 0;

    void Start()
    {
        simplex = GetComponent<SimplexNoise>();
        simplex.init(seed);
        offsetX = Random.Range(0f, 9999f);
        offsetY = Random.Range(0f, 9999f);
        simplex.offsetX = offsetX;
        simplex.offsetY = offsetY;
        simplex.scale = scale;
    }

    public virtual void GenerateTerrain()
    {
        Terrain terrain = GetComponent<Terrain>();
        terrain.terrainData = GenerateTerrain(terrain.terrainData);
        terrain.Flush();
    }

    void Update()
    {
        GenerateTerrain();
    }

    TerrainData GenerateTerrain(TerrainData terrainData)
    {
        terrainData.heightmapResolution = width + 1;
        terrainData.size = new Vector3(width, depth, height);
        terrainData.SetHeightsDelayLOD(0, 0, GenerateHeights());
        if(terrainTexture)
        {
            SplatPrototype[] textures = new SplatPrototype[1];
			textures [0] = new SplatPrototype ();
            textures[0].texture = terrainTexture;
            textures[0].tileSize = new Vector2(1,1);   
            terrainData.splatPrototypes = textures;
        }
        return terrainData;
    }

    float[,] GenerateHeights()
    {
		GenerationController con = GetComponent<GenerationController> ();
		return con.heightMap.Values;

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