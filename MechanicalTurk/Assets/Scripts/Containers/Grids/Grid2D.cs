using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Shane McDermott 2018

public struct Grid2D
{
    public int width;
    public int height;
    private Vector2[,] vertices;

    public Grid2D(int width, int height)
    {
        this.width = width;
        this.height = height;
        this.vertices = new Vector2[width, height];
    }

    public int GetWidth()
    {
        return width;
    }

    public int GetHeight()
    {
        return height;
    }

    public bool IsValidCoord(IntPoint gridCoord)
    {
        return gridCoord.GreaterThan(new IntPoint(-1, -1)) && gridCoord.LessThan(new IntPoint(width, height));
    }

    public Vector2 GetPoint(int xCoord, int yCoord)
    {
        return GetPoint(new IntPoint(xCoord, yCoord));
    }

    public Vector2 GetPoint(IntPoint gcoord)
    {
        if (IsValidCoord(gcoord))
            return vertices[gcoord.x, gcoord.y];

        return Vector2.positiveInfinity;
    }

    public void SetPoint(int x, int y, Vector2 point)
    {
        SetPoint(new IntPoint(x, y), point);
    }

    public void SetPoint(IntPoint gcoord, Vector2 point)
    {
        if(IsValidCoord(gcoord))
        {
            vertices[gcoord.x, gcoord.y] = point;
        }
    }

    public void SquareAroundPoint(IntPoint point, int squareSize, out Vector2[,] square)
    {
        square = new Vector2[squareSize, squareSize];

        point.x = Mathf.Clamp(point.x - (squareSize / 2), 0, width - 1);
        point.y = Mathf.Clamp(point.y - (squareSize / 2), 0, height - 1);
        for (int x = 0; x < squareSize; x++)
        {
            for(int y = 0; y < squareSize; y++)
            {
                square[x,y] = GetPoint(point.x + x, point.y+y);
            }
        }
    }
}
