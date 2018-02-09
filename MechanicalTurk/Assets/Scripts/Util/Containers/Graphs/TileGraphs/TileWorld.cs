using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Shane McDermott 2017
//NodeArray implementation for tile based AStar
public class TileWorld : MonoBehaviour
{
    public TileGraph tileGraph;
    public bool drawRecords = false;
    public bool drawBlocking = false;

    public TileRecord[,] nodeArray;
    private IntPoint numTiles;
    private Vector3 tileDimensions;

    public void Awake()
    {
        Init();
    }


    public void Init()
    {
        tileGraph.Init();
        numTiles = tileGraph.WorldToTile(tileGraph.graphSize);
        tileDimensions = new Vector3(tileGraph.tileSize, tileGraph.tileSize, tileGraph.tileSize);
        nodeArray = new TileRecord[numTiles.x, numTiles.y];
        ClearNodes();
    }


    //Resets all of the node records.
    public void ClearNodes()
    {
        for (IntPoint tile = new IntPoint(0, 0); tile.x < numTiles.x; tile.x++)
        {
            for (tile.y = 0; tile.y < numTiles.y; tile.y++)
            {
                nodeArray[tile.x, tile.y] = new TileRecord();
                nodeArray[tile.x, tile.y].category = NodeCategory.Unvisited;
            }
        }

    }

    public void SetNodeCategory(IntPoint node, NodeCategory category)
    {
        nodeArray[node.x, node.y].category = category;
    }

    public TileRecord GetRecordAt(IntPoint node)
    {
        return nodeArray[node.x, node.y];
    }

    public void OnDrawGizmos()
    {
        if(drawRecords)
        {
            for (IntPoint tile = new IntPoint(0, 0); tile.x < numTiles.x; tile.x++)
            {
                for (tile.y = 0; tile.y < numTiles.y; tile.y++)
                {
                    Vector3 worldLoc = tileGraph.TileToWorld(tile);
                   // worldLoc.y = tileGraph.heightValues[tile.x, tile.y];
                    
                    if(nodeArray[tile.x,tile.y].category == NodeCategory.Open)
                    {
                        Gizmos.color = Color.blue;
                        Gizmos.DrawCube(worldLoc, tileDimensions);
                    }
                    else if(nodeArray[tile.x,tile.y].category == NodeCategory.Closed)
                    {
                        Gizmos.color = Color.gray;
                        Gizmos.DrawCube(worldLoc, tileDimensions);
                    }
                    Gizmos.color = Color.black;
                    Gizmos.DrawWireCube(worldLoc, tileDimensions);

                }
            }

        }
        if(drawBlocking)
        {
            for (IntPoint tile = new IntPoint(0, 0); tile.x < numTiles.x; tile.x++)
            {
                for (tile.y = 0; tile.y < numTiles.y; tile.y++)
                {
                    if (tileGraph.IsTileBlocked(tile))
                    {
                        Vector3 worldLoc = tileGraph.TileToWorld(tile);
                        //worldLoc.y = tileGraph.heightValues[tile.x, tile.y];
                        Gizmos.color = Color.red;
                        Gizmos.DrawCube(worldLoc, tileDimensions);
                    }

                }
            }
        }
    }
}
