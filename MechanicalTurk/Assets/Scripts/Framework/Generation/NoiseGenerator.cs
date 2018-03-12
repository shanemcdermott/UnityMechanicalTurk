using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NoiseGenerator : GenerationAlgorithm
{
    [SerializeField]
    public NoiseMap noiseMap;

    public float scale
    {
        get { return noiseMap.scale; }
        set
        {
            noiseMap.scale = value;
        }
    }

    public int width
    {
        get { return noiseMap.Width; }
        set
        {
            noiseMap.Width = value;
        }
    }

    public int height
    {
        get { return noiseMap.Height; }
        set
        {
            noiseMap.Height = value;
        }
    }

    public override void Setup()
    {
        if (noiseMap == null)
            noiseMap = GetComponent<NoiseMap>();
    }

    public override bool CanGenerate()
    {
        return noiseMap != null;
    }

    public override void Generate()
    {
        noiseMap.Values = GenerateHeightMap(noiseMap.Width, noiseMap.Height);
    }
    public abstract float[,] GenerateHeightMap(int mapWidth, int mapHeight);
}
