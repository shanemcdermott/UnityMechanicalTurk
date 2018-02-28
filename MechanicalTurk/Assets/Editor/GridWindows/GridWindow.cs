using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;



public abstract class GridWindow
{
    public PolyGrid grid = null;
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

    public void SetGrid(PolyGrid grid)
    {
        this.grid = grid;
    }

    public abstract void Subdivide();
}
