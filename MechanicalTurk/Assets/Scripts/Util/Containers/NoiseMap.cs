using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 2D Array with indexers
/// </summary>
[System.Serializable]
public class NoiseMap : MonoBehaviour
{
    public Vector2Int Dimensions
    {
        get { return new Vector2Int(width, height); }
        set
        {
            Resize(value);
        }
    }

    public int Width
    {
        get { return width; }
        set
        {
            if (width != value)
            {
                width = value;
                heightmap = new float[width, height];
            }
        }
    }

 
    public int Height
    {
        get { return height; }
        set
        {
            if (height != value)
            {
                height = value;
                heightmap = new float[width, height];
            }
        }
    }

    [SerializeField]
    private int width = 256;
    [SerializeField]
    private int height = 256;


    private float[,] heightmap;

    public float[,] Values
    {
        get { return heightmap; }
        set
        {
            heightmap = value;
            width = heightmap.GetLength(0);
            height = heightmap.GetLength(1);
        }
    }

    public float this[int i]
    {
        get { return heightmap[i / width, i % height]; }
        set
        {
            heightmap[i / width, i % height] = value;
        }
    }

    public float this[Vector2Int v2]
    {
        get { return heightmap[v2.x, v2.y]; }
        set
        {
            heightmap[v2.x, v2.y] = value;
        }
    }

    public float this[int x, int y]
    {
        get { return heightmap[x, y]; }
        set
        {
            heightmap[x, y] = value;
        }
    }

    public void Resize(Vector2Int newSize)
    {
        width = newSize.x;
        height = newSize.y;
        heightmap = new float[width, height];
    }

    public int ToIndex(int x, int y)
    {
        return x * width + y;
    }

    public int GetWidth()
    {
        return width;
    }

    public int GetHeight()
    {
        return height;
    }
}
