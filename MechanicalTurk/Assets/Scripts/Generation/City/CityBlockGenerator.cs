using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CityBlockGenerator : BlockGenerator
{

    public override void Generate()
    {
        SubdivideGrid();
        PopulateBlocks();
    }

    public virtual void SubdivideGrid()
    {
        Vector3 lotSize = Dimensions * 0.5f;
        /*Split the grid in half until the desired leaf size is achieved*/
        while (lotSize.x >= MinLotSize.x && lotSize.z >= MinLotSize.z)
        {
            districtNode.Subdivide();
            lotSize *= 0.5f;
        }
    }
}
