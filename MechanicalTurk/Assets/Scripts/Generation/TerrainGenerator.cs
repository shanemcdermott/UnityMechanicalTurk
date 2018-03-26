using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : GenerationAlgorithm
{
    public NoiseGenerator heightMapGenerator;
    public BiomeGenerator biomeGenerator;

    public float heightScale = 1f;
    public NoiseMap heightMap;
    public Terrain terrain;
    public Texture2D biomeTexture;
	public Texture2D roadTexture;

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
    }

    public override void Generate()
    {
        heightMapGenerator.OnGenerationComplete.AddListener(GenerateBiomes);
        heightMapGenerator.OnGenerationComplete.AddListener(ApplyHeightMap);
        heightMapGenerator.Setup();
        heightMapGenerator.Generate(true);
    }

    public virtual void GenerateBiomes()
    {
        biomeGenerator.OnGenerationComplete.AddListener(ApplyBiomeMap);
        biomeGenerator.Setup();
        biomeGenerator.Generate(true);
    }

    /// <summary>
    /// Assign height values to the terrain
    /// </summary>
    public virtual void ApplyHeightMap()
    {
        TerrainData terrainData = terrain.terrainData;
        terrainData.heightmapResolution = heightMap.Width + 1;
        terrainData.size = new Vector3(heightMap.Width, heightScale, heightMap.Height);
        terrainData.SetHeightsDelayLOD(0, 0, heightMap.Values);
    }


    public virtual void ApplyBiomeMap()
    {
        if (biomeTexture != null)
        {
            biomeTexture.Resize(heightMap.Width, heightMap.Height);
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
            SplatPrototype[] textures = new SplatPrototype[2];
            textures[0] = new SplatPrototype();
            textures[0].texture = biomeTexture;
            textures[0].tileSize = heightMap.Dimensions;
			textures [1] = new SplatPrototype ();
			textures [1].texture = roadTexture;
			textures [1].tileSize = new Vector2Int (1, 1);
            terrain.terrainData.splatPrototypes = textures;
        }
		float[,,] alpha = new float[heightMap.Width, heightMap.Height, 2];
		for (int x = 0; x < heightMap.Width; x++) {
			for (int y = 0; y < heightMap.Height; y++) {
				alpha [x / heightMap.Width, y / heightMap.Height, 0] = 0;
				alpha [x / heightMap.Width, y / heightMap.Height, 1] = 1;
			}
		}
		terrain.terrainData.SetAlphamaps (0, 0, alpha);
        terrain.terrainData.RefreshPrototypes();
        terrain.Flush();
    }
}
