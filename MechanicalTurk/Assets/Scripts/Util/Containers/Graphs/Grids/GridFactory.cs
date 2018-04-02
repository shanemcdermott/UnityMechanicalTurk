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

    public SquareGridParams(PolyGrid source)
    {
        Dimensions = source.Dimensions;
        FacesPerSide = source.FacesPerSide;
    }

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
        grid.Dimensions = gridParams.Dimensions;
        grid.FacesPerSide = gridParams.FacesPerSide;
        return PopulateSquareGrid(ref grid);
    }

    public static bool PopulateSquareGrid(ref PolyGrid grid)
    {
        SquareGridParams gridParams = new SquareGridParams(grid);
        Vector2 faceDimensions = gridParams.GetFaceDimensions();
        Vector2Int vertsPerSide = gridParams.NumVertices();

        Node[,] vertices = new Node[vertsPerSide.x, vertsPerSide.y];
        GridFace[,] faces = new GridFace[gridParams.FacesPerSide.x, gridParams.FacesPerSide.y];


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
                Vector2 vertex = MathOps.Midpoint(vertices[x, y].GetPositionXZ(), vertices[x + 1, y + 1].GetPositionXZ());
                faces[x, y] = new GridFace(vertex);
                faces[x, y].AddVertices(new Node[] { vertices[x,y], vertices[x+1,y], vertices[x,y+1], vertices[x+1,y+1]});
                grid.AddFace(faces[x, y]);
                
            }
        }

        return grid != null;
    }

    public static void GeneratePerlinGrid(ref PolyGrid grid, int low, int high)
    {
        SquareGridParams gridParams = new SquareGridParams(grid);
        Vector2 faceDimensions = gridParams.GetFaceDimensions();
        Vector2Int vertsPerSide = gridParams.NumVertices();

        Node[,] vertices = new Node[vertsPerSide.x, vertsPerSide.y];
        GridFace[,] faces = new GridFace[gridParams.FacesPerSide.x * high, gridParams.FacesPerSide.y * high];

        for (int x = 0; x < vertsPerSide.x; x++)
        {
            for (int y = 0; y < vertsPerSide.y; y++)
            {
                Vector2 vertex = new Vector2(faceDimensions.x * x, faceDimensions.y * y);
                
                vertices[x, y] = new Node(vertex);
                
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
        for (int x = 0; x < vertsPerSide.x; x++)
        {
            for (int y = 0; y < vertsPerSide.y; y++)
            {
                Vector3 vertex = vertices[x, y].GetPosition();

                //random roads
                int roadNumberEastWest = Random.Range(low, high + 1);
                int roadNumberNorthSouth = Random.Range(low, high + 1);
                
                float intervalEastWest = faceDimensions.x / roadNumberEastWest;
                float intervalNorthSouth = faceDimensions.y / roadNumberNorthSouth;

                Debug.Log("randomEW: " + roadNumberEastWest + " ns " + roadNumberNorthSouth + " intEW " + intervalEastWest + " ns " + intervalNorthSouth + " low " + low + " hi " + high);
                //East/West Roads
                for (int i = 1; i < roadNumberEastWest; i++)
                {
                    Vector2 randomVertexWest = new Vector2(vertex.x, vertex.z + (intervalEastWest * i));
                    Vector2 randomVertexEast = new Vector2(vertex.x + faceDimensions.x, vertex.z + (intervalEastWest * i));
                    grid.AddFace(new GridFace(randomVertexWest));
                    grid.AddFace(new GridFace(randomVertexEast));
                }

                //North/South Roads
                for (int i = 1; i < roadNumberNorthSouth; i++)
                {
                    Vector2 randomVertexSouth = new Vector2(vertex.x + (intervalNorthSouth * i), vertex.z);
                    Vector2 randomVertexNorth = new Vector2(vertex.x + (intervalNorthSouth * i), vertex.z + faceDimensions.y);
                    grid.AddFace(new GridFace(randomVertexSouth));
                    grid.AddFace(new GridFace(randomVertexNorth));
                }
            }
        }
    }
}
