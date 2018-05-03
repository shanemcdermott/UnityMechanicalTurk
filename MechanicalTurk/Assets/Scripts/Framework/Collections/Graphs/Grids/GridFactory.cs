using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Collections
{
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
            return FacesPerSide + new Vector2Int(1, 1);
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
            GridNode[,] faces = new GridNode[gridParams.FacesPerSide.x, gridParams.FacesPerSide.y];

            //Add Vertices
            for (int x = 0; x < vertsPerSide.x; x++)
            {
                for (int y = 0; y < vertsPerSide.y; y++)
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

            GridFaceFactory<GridNode> faceFactory = new GridFaceFactory<GridNode>();
            //Create Faces
            for (int x = 0; x < gridParams.FacesPerSide.x; x++)
            {
                for (int y = 0; y < gridParams.FacesPerSide.y; y++)
                {
                    Vector2 vertex = Vector2.Lerp(vertices[x, y].GetPositionXZ(), vertices[x + 1, y + 1].GetPositionXZ(), 0.5f);
                    faces[x, y] = faceFactory.GetNewGridFace(vertex, new Node[] { vertices[x, y], vertices[x + 1, y], vertices[x, y + 1], vertices[x + 1, y + 1] });
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

            Dictionary<Vector2, Node> faceVertices = new Dictionary<Vector2, Node>();

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

                            Vector2 centerVert = Vector2.Lerp(bottomLeft, topRight,0.5f);

                            Vector2 bottomRight = new Vector2(vertex.x + ((localx + 1) * intervalEastWest), vertex.z + (z * intervalNorthSouth));
                            Vector2 topLeft = new Vector2(vertex.x + (localx * intervalEastWest), vertex.z + ((z + 1) * intervalNorthSouth));

                            Node bottomLeftNode;
                            Node topRightNode;
                            Node bottomRightNode;
                            Node topLeftNode;

                            if (!faceVertices.TryGetValue(bottomLeft, out bottomLeftNode))
                            {
                                bottomLeftNode = new Node(new Vector3(bottomLeft.x, 0, bottomLeft.y));
                                faceVertices.Add(bottomLeft, bottomLeftNode);
                            }

                            if (!faceVertices.TryGetValue(topRight, out topRightNode))
                            {
                                topRightNode = new Node(new Vector3(topRight.x, 0, topRight.y));
                                faceVertices.Add(topRight, topRightNode);
                            }

                            if (!faceVertices.TryGetValue(bottomRight, out bottomRightNode))
                            {
                                bottomRightNode = new Node(new Vector3(bottomRight.x, 0, bottomRight.y));
                                faceVertices.Add(bottomRight, bottomRightNode);
                            }

                            if (!faceVertices.TryGetValue(topLeft, out topLeftNode))
                            {
                                topLeftNode = new Node(new Vector3(topLeft.x, 0, topLeft.y));
                                faceVertices.Add(topLeft, topLeftNode);
                            }

                            List<Node> vertList = new List<Node>(
                                new Node[]{
                                bottomLeftNode, bottomRightNode, topLeftNode, topRightNode
                                }
                            );

                            GridNode gridNode = new GridNode(new Vector3(centerVert.x, 0, centerVert.y), ref vertList);

                            gridNode.ConnectVertices();
                            grid.AddFace(gridNode);
                        }
                    }
                }
            }
        }
    }
}