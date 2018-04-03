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

        GridFaceFactory<GridFace> faceFactory = new GridFaceFactory<GridFace>();
        //Create Faces
        for (int x = 0; x < gridParams.FacesPerSide.x; x++)
        {
            for (int y = 0; y < gridParams.FacesPerSide.y; y++)
            {
                Vector2 vertex = MathOps.Midpoint(vertices[x, y].GetPositionXZ(), vertices[x + 1, y + 1].GetPositionXZ());
                faces[x, y] = faceFactory.GetNewGridFace(vertex, new Node[] { vertices[x,y], vertices[x+1,y], vertices[x,y+1], vertices[x+1,y+1]});
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
        for (int x = 0; x < grid.FacesPerSide.x; x++)
        {
            for (int y = 0; y < grid.FacesPerSide.y; y++)
            {
                Vector3 vertex = vertices[x, y].GetPosition();

                //random roads
                int roadNumberEastWest = Random.Range(low, high + 1);
                int roadNumberNorthSouth = Random.Range(low, high + 1);

                float intervalEastWest = faceDimensions.x / roadNumberEastWest;
                float intervalNorthSouth = faceDimensions.y / roadNumberNorthSouth;

                
                for (int localx = 0; localx < roadNumberEastWest; localx++)
                {
                    for (int z = 0; z < roadNumberNorthSouth; z++)
                    {
                        Vector2 bottomLeft = new Vector2(vertex.x + (localx * intervalEastWest), vertex.z + (z * intervalNorthSouth));
                        Vector2 topRight = new Vector2(vertex.x + ((localx + 1) * intervalEastWest), vertex.z + ((z + 1) * intervalNorthSouth));
                        
                        Vector2 centerVert = MathOps.Midpoint(bottomLeft, topRight);
                        GridFace gridface = new GridFace(centerVert);

                        Vector2 bottomRight = new Vector2(vertex.x + ((localx + 1) * intervalEastWest), vertex.z + (z * intervalNorthSouth));
                        Vector2 topLeft = new Vector2(vertex.x + (localx * intervalEastWest), vertex.z + ((z + 1) * intervalNorthSouth));

                        gridface.AddVertices(new Node[] {new Node(bottomLeft), new Node(bottomRight), new Node(topLeft), new Node(topRight)});
                        grid.AddFace(gridface);
                    }
                }
            }
        }
    }
}
