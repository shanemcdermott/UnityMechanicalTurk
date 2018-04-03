using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CityGenerator : GenerationAlgorithm
{
    public NoiseMap heightMap;

    public PolyGrid polyGrid;



    public override bool CanGenerate()
    {
        return heightMap != null;
    }

    public override void Setup()
    {
        if (polyGrid == null)
        {
            polyGrid = gameObject.GetComponent<PolyGrid>();
            if (polyGrid == null)
            {
                polyGrid = gameObject.AddComponent<PolyGrid>();
                polyGrid.Dimensions = heightMap.Dimensions;
                polyGrid.FacesPerSide = new Vector2Int(2, 2);
            }
        }
    }


}
