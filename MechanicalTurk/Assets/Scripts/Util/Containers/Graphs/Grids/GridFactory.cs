using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum GridType
{
    Triangle = 0,
    Square = 1,
    Polygon = 2
}

public struct SquareGridParams
{
    public Vector2 Dimensions;
    public Vector2Int FacesPerSide;

    public Vector2 GetFaceDimensions()
    {
        return new Vector2(Dimensions.x / FacesPerSide.x, Dimensions.y / FacesPerSide.y);
    }

    public Vector2Int NumVertices()
    {
        return FacesPerSide + new Vector2Int(1,1);
    }
}

public class GridFactory
{

    public static bool CreateSquareGrid(SquareGridParams gridParams, out PolyGrid grid)
    {

        GameObject go = new GameObject("Grid");
        grid = go.AddComponent<PolyGrid>();
        Vector2 faceDimensions = gridParams.GetFaceDimensions();
        Vector2Int vertsPerSide = gridParams.NumVertices();

        Node[,] vertices = new Node[vertsPerSide.x, vertsPerSide.y];
        Node[,] faces = new Node[gridParams.FacesPerSide.x, gridParams.FacesPerSide.y];


        //Add Vertices
        for(int x = 0; x < vertsPerSide.x; x++)
        {
            for(int y = 0; y < vertsPerSide.y; y++)
            {
                Vector2 vertex = new Vector2(faceDimensions.x * x, faceDimensions.y * y);
                vertices[x, y] = new Node(vertex);

                //Add vertex to grid
                grid.AddVertex(vertices[x, y]); 
            }
        }
        //Connect Vertices
        for (int x = 0; x < vertsPerSide.x; x++)
        {
            for (int y = 0; y < vertsPerSide.y; y++)
            {
                if (x < vertsPerSide.x - 1)
                {
                    vertices[x, y].AddConnection(vertices[x + 1, y]);
                }
                if (y < vertsPerSide.y - 1)
                {
                    vertices[x, y].AddConnection(vertices[x, y + 1]);
                }
            }
        }

        //Create Faces
        for (int x = 0; x < gridParams.FacesPerSide.x; x++)
        {
            for (int y = 0; y < gridParams.FacesPerSide.y; y++)
            {
                Vector2 vertex = MathOps.Midpoint(vertices[x, y].GetPosition(), vertices[x + 1, y + 1].GetPosition());
                faces[x, y] = new Node(vertex);
                grid.AddFace(faces[x, y]);
                
            }
        }

        //Connect Faces
        for (int x = 0; x < gridParams.FacesPerSide.x; x++)
        {
            for (int y = 0; y < gridParams.FacesPerSide.y; y++)
            {
                if (x < gridParams.FacesPerSide.x - 1)
                {
                    faces[x, y].AddConnection(faces[x + 1, y]);
                }
                if (y < gridParams.FacesPerSide.y - 1)
                {
                    faces[x, y].AddConnection(faces[x, y + 1]);
                }
            }
        }

        return grid != null;
    }

}
