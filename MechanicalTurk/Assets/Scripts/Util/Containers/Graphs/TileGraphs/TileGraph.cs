using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGraph : MonoBehaviour, IGraph<IntPoint>
{
    //Size of each tile
    public float tileSize = 1.0f;
    public Vector3 graphSize = new Vector3(40, 1, 40);
    public string[] blocking = new string[1];
    public float[,] heightValues;

    public Terrain terrain;

    private Vector3 tileExtents;


    public void Awake()
    {
        Init();
    }

    public void Init()
    {
        tileExtents = new Vector3(tileSize * 0.5f, tileSize * 0.5f, tileSize * 0.5f);
        blocking[0] = "Blocking";
        InitHeightField();
    }

    
    //If a terrain reference is provided, samples the height value at each point in the graph.
    //Otherwise sets each height value equal to the graph's transform.
    public void InitHeightField()
    {
        IntPoint numTiles = WorldToTile(graphSize);
        heightValues = new float[numTiles.x, numTiles.y];

        for (IntPoint tile = new IntPoint(0, 0); tile.x < numTiles.x; tile.x++)
        {
            for (tile.y = 0; tile.y < numTiles.y; tile.y++)
            {
                
                if(terrain != null)
                    heightValues[tile.x, tile.y] = terrain.SampleHeight(TileToWorld(tile)); 
                else
                    heightValues[tile.x, tile.y] = transform.position.y;
                //if (heightValues[tile.x, tile.y] != 0)
                //    Debug.Log("Tile " + tile + " has a height of " + heightValues[tile.x, tile.y]);
            }
        }
    }

    public IntPoint WorldToTile(Vector2 worldPosition)
    {
        return new IntPoint(worldPosition.x / tileSize, worldPosition.y / tileSize);
    }

    public IntPoint WorldToTile(Vector3 worldPosition)
    {
        return new IntPoint(worldPosition.x / tileSize, worldPosition.z / tileSize);
    }

    public Vector3 TileToWorld(IntPoint tilePosition)
    {
        return new Vector3(tilePosition.x * tileSize, heightValues[tilePosition.x, tilePosition.y], tilePosition.y * tileSize);

        //return new Vector3(tilePosition.x * tileSize, transform.position.y, tilePosition.y * tileSize);
    }

    public Vector3 WorldToTileCenter(Vector3 worldPosition)
    {
        return TileToWorld(WorldToTile(worldPosition));
    }

    /// <summary>
    /// Fills connections with node connections that can be reached from fromNode
    /// </summary>
    /// <param name="fromNode">The tile coordinates of the node to find connections for.</param>
    /// <param name="connections">The resulting set of connections</param>
    public void GetConnections(IntPoint fromNode, out List<IConnection<IntPoint>> connections)
    {
        connections = new List<IConnection<IntPoint>>();

        float fromNodeHeight = heightValues[fromNode.x, fromNode.y];
        if (fromNodeHeight == 0)
            fromNodeHeight = 0.01f;
        for (int x = -1; x <= 1; x += 1)
        {
            for (int y = -1; y <= 1; y += 1)
            {
                if (x == 0 && y == 0) continue;

                IntPoint tile = IntPoint.Add(fromNode, x, y);
              
                if(!IsTileBlocked(tile))
                {
                    BaseConnection<IntPoint> connection = new BaseConnection<IntPoint>(fromNode, tile);

                    connection.cost = Vector3.Distance(TileToWorld(fromNode), TileToWorld(tile));
                    /*connection.cost = IntPoint.Distance(fromNode, tile);
                    if (terrainData != null)
                    {

                        float gradient = heightValues[tile.x, tile.y] / fromNodeHeight;
                        connection.cost *= gradient;
                    }
                    */
                    connections.Add(connection);
                }
                    
            }

        }
    }


    //Checks to see if the given tile has anything blocking its path.
    public bool IsTileBlocked(IntPoint tileCoords)
    {
        Vector3 worldNode = TileToWorld(tileCoords);
        Collider[] hits = Physics.OverlapBox(worldNode, tileExtents, Quaternion.identity, LayerMask.GetMask(blocking));
        return hits.Length > 0;
    }

}
