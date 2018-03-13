using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiomeGenerator : GenerationAlgorithm
{

    public TerrainType[] biomeTypes;

    public NoiseMap heightMap;

    public int[,] biomeMap;

    public override bool CanGenerate()
    {
        return biomeTypes != null &&
            biomeTypes.Length > 0 &&
            heightMap != null;
    }

    public override void Setup()
    {
        if(heightMap == null)
        {
            heightMap = GetComponent<NoiseMap>();
        }
    }

    public override void Generate()
    {
        biomeMap = new int[heightMap.Width, heightMap.Height];
        for(int x = 0; x < heightMap.Width; x++)
        {
            for(int y = 0; y < heightMap.Height; y++)
            {
                biomeMap[x, y] = FindBestTerrain(heightMap[x, y]);
            }
        }
    }

    public Color[] GetColorMap()
    {
        Color[] colorMap = new Color[heightMap.Width * heightMap.Height];
        for(int x = 0; x < heightMap.Width; x++)
        {
            for(int y = 0; y < heightMap.Height; y++)
            {
                colorMap[heightMap.ToIndex(x, y)] = biomeTypes[biomeMap[x, y]].colour;
            }
        }

        return colorMap;
    }

    public int FindBestTerrain(float heightValue)
    {
        for(int i = 0; i < biomeTypes.Length; i++)
        {
            if (heightValue <= biomeTypes[i].height)
                return i;
        }

        return 0;
    }
}
