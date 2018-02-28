using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NoiseAlgorithm
{
    Simplex,
    Perlin
}

public interface NoiseGenerator
{
   float[,] GenerateHeightmap(int width, int height);
}
