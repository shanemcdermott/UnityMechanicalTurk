using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;



public abstract class GridWindow
{
    protected GridType type = GridType.Square;
    protected string name = "Square";

    public string GetName()
    {
        return name;
    }
    public GridType GetGridType()
    {
        return type;
    }

    public abstract void ShowParams();

    public abstract void CreateGrid(out PolyGrid grid);

}
