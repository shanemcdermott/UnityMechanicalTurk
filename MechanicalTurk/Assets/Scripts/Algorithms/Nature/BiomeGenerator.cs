using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using Framework.Collections;

namespace Framework.Generation
{

    [System.Serializable]
    public struct TerrainType
    {
        public string name;
        public float height;
        public Color colour;

        public TerrainType(string terrainName, float  heightRequirement, Color terrainColor)
        {
            name = terrainName;
            height = heightRequirement;
            colour = terrainColor;
        }
    }

    public class BiomeGenerator : GenerationAlgorithm
    {

        public TerrainType[] biomeTypes = new TerrainType[4]
                {
                    new TerrainType("dirt", 0.2f, new Color(83/255f,66/255f,11/255f)),
                    new TerrainType("swamp", 0.4f, new Color(15/255f,62/255f,14/255f)),
                    new TerrainType("grass", 0.6f, new Color(61/255f,167/255f,62/255f)),
                    new TerrainType("stone", 0.8f, new Color(98/255f,98/255f,98/255f)),
                };

        public NoiseMap heightMap
        {
            get
            {
                if(_heightMap == null)
                {
                    _heightMap = GetComponent<NoiseMap>();
                }
                return _heightMap;
            }
            set
            {
                _heightMap = value;
            }
        }
        private NoiseMap _heightMap;

        public int[,] biomeMap;

        public override bool CanGenerate()
        {
            return biomeTypes != null &&
                biomeTypes.Length > 0 &&
                heightMap != null;
        }

        public override void Setup()
        {
            if(biomeTypes == null || biomeTypes.Length==0)
            {
                biomeTypes = new TerrainType[4]
                {
                    new TerrainType("dirt", 0.2f, new Color(83/255f,66/255f,11/255f)),
                    new TerrainType("swamp", 0.4f, new Color(15/255f,62/255f,14/255f)),
                    new TerrainType("grass", 0.6f, new Color(61/255f,167/255f,62/255f)),
                    new TerrainType("stone", 0.8f, new Color(98/255f,98/255f,98/255f)),
                };
            }
        }

        public override void Generate()
        {
            biomeMap = new int[heightMap.Width, heightMap.Height];
            for (int x = 0; x < heightMap.Width; x++)
            {
                for (int y = 0; y < heightMap.Height; y++)
                {
                    biomeMap[x, y] = FindBestTerrain(heightMap[x, y]);
                }
            }
        }

        public Color[] GetColorMap()
        {
            Color[] colorMap = new Color[heightMap.Width * heightMap.Height];
            for (int x = 0; x < heightMap.Width; x++)
            {
                for (int y = 0; y < heightMap.Height; y++)
                {
                    colorMap[heightMap.ToIndex(x, y)] = biomeTypes[biomeMap[x, y]].colour;
                }
            }

            return colorMap;
        }

        public int FindBestTerrain(float heightValue)
        {
            for (int i = 0; i < biomeTypes.Length; i++)
            {
                if (heightValue <= biomeTypes[i].height)
                    return i;
            }

            return 0;
        }

    }
}