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

    public virtual Vector3 GetCityCenter()
    {
        return transform.root.position + new Vector3(heightMap.Width * 0.5f, 20f, heightMap.Height * 0.5f);
    }
}
