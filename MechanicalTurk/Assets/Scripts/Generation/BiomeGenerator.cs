using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct TerrainType
{
    public string name;
    public float height;
    public Color colour;
}

public class BiomeGenerator : GenerationAlgorithm
{
   
    /// <summary>
    /// Source heightmap for region assignment
    /// </summary>
    public NoiseMap noiseMap;

    public TerrainType[] regions;

    public Color[] colorMap;

    public override bool CanGenerate()
    {
        return noiseMap != null;
    }

    public override void Setup()
    {
        colorMap = new Color[noiseMap.Width * noiseMap.Height];
    }

    public override void Generate()
    {
        for (int x = 0; x < noiseMap.Width; x++)
        {
            for (int y = 0; y < noiseMap.Height; y++)
            {
                float currentHeight = noiseMap[x, y];
                for (int i = 0; i < regions.Length; i++)
                {
                    if (currentHeight <= regions[i].height)
                    {
                        colorMap[noiseMap.ToIndex(x,y)] = regions[i].colour;
                        break;
                    }
                }
            }
        }
    }
}
