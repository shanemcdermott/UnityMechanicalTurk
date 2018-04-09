using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CityGenerator : GenerationAlgorithm
{
    public NoiseMap heightMap;

    public override bool CanGenerate()
    {
        return heightMap != null;
    }

    public override void Setup()
    {
        if (heightMap == null)
        {
            heightMap = gameObject.GetComponent<NoiseMap>();
        }
    }


}
