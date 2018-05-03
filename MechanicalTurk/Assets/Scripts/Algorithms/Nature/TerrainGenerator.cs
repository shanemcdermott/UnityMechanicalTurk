using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using Framework.Collections;
using Framework.Util;

namespace Framework.Generation
{

    public class TerrainGenerator : GenerationAlgorithm
    {
        public NoiseGenerator heightMapGenerator;
        public BiomeGenerator biomeGenerator;

        public float heightScale = 1f;
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
            if (heightMap == null)
            {
                heightMap = GetComponent<NoiseMap>();
            }
            if (heightMapGenerator == null)
            {
                heightMapGenerator = GetComponent<NoiseGenerator>();
            }
            if (biomeGenerator == null)
            {
                biomeGenerator = GetComponent<BiomeGenerator>();
            }
            if (terrain == null)
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
                biomeTexture = TextureFactory.TextureFromColourMap(biomeGenerator.GetColorMap(), heightMap.Width, heightMap.Height);
                biomeTexture.name = "BiomeTexture";
            }
            if (biomeTexture)
            {
                SplatPrototype[] textures = new SplatPrototype[1];
                textures[0] = new SplatPrototype();
                textures[0].texture = biomeTexture;
                textures[0].tileSize = heightMap.Dimensions;
                terrain.terrainData.splatPrototypes = textures;
            }
            terrain.terrainData.RefreshPrototypes();
            terrain.Flush();
        }
    }
}