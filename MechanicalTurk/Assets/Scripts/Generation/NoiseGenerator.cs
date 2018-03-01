using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NoiseGenerator : MonoBehaviour
{
	public abstract void init (int seed);
	public abstract float[,] GenerateHeightMap(int width, int height);
}
