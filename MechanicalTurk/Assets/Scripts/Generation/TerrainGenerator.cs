using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : GenerationAlgorithm
{
    public int depth = 3;

    public NoiseGenerator heightMapGenerator;
    public BiomeGenerator biomeGenerator;

    public NoiseMap heightMap;
    public Terrain terrain;
    public Texture2D biomeTexture;
   

    public override bool CanGenerate()
    {
        return heightMapGenerator != null && 
            biomeGenerator != null &&
            heightMap != null && 
            terrain != null;
    }

    public override void Setup()
    {
        if(heightMap == null)
        {
            heightMap = GetComponent<NoiseMap>();
        }
        if(heightMapGenerator == null)
        {
            heightMapGenerator = GetComponent<NoiseGenerator>();
        }
        if(biomeGenerator == null)
        {
            biomeGenerator = GetComponent<BiomeGenerator>();
        }
        if(terrain == null)
        {
            terrain = GetComponent<Terrain>();
        }
        heightMapGenerator.noiseMap = heightMap;
        biomeGenerator.heightMap = heightMap;
    }

    public override void Generate()
    {
        heightMapGenerator.OnGenerationComplete.AddListener(GenerateBiomes);
        heightMapGenerator.Generate(true);
    }

    public virtual void GenerateBiomes()
    {
        biomeGenerator.OnGenerationComplete.AddListener(GenerateTerrain);
        biomeGenerator.heightMap = heightMap;
        biomeGenerator.Generate(true);
    }

    public virtual void GenerateTerrain()
    {
        TerrainData terrainData = terrain.terrainData;
        terrainData.heightmapResolution = heightMap.Width + 1;
        terrainData.size = new Vector3(heightMap.Width, depth, heightMap.Height);
        terrainData.SetHeightsDelayLOD(0, 0, heightMap.Values);
        if (biomeTexture != null)
        {
            biomeTexture.SetPixels(biomeGenerator.GetColorMap());
            biomeTexture.Apply();
        }
        else
        {
            biomeTexture = TextureGenerator.TextureFromColourMap(biomeGenerator.GetColorMap(), heightMap.Width, heightMap.Height);
            biomeTexture.name = "BiomeTexture";
        }
        if (biomeTexture)
        {
            SplatPrototype[] textures = new SplatPrototype[1];
            textures[0] = new SplatPrototype();
            textures[0].texture = biomeTexture;
            textures[0].tileSize = heightMap.Dimensions;
            terrainData.splatPrototypes = textures;
        }
        terrain.terrainData.RefreshPrototypes();
        terrain.Flush();
    }

}
