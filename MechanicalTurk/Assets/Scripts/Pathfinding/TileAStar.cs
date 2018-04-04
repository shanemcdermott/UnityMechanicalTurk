using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Shane McDermott 2017

public class TileAStar : MonoBehaviour
{
    public PathSmoother pathSmoother;
    public bool drawUnsmoothedPath = false;

    //AStar Parameters
    public TileWorld tileWorld;
    public TileGraph tileGraph;
    public Transform startTransform;
    public Transform goalTransform;
    public Heuristic<IntPoint> heuristic;

    //Path 
    private List<Vector3> path;
    private List<Vector3> smoothedPath;

    //Tile translations of world positions
    private IntPoint startNode;
    private IntPoint goalNode;
    private IntPoint currentNode;

    private List<IntPoint> priorityNodes;
    private int numOpenNodes;

    //Assigns starting point and goal based on parameters 
    //before calling the pathfinding function.
    //Returns true if a function was found.
    public bool FindAgentPath(GameObject agent, Transform goal)
    {
        startTransform = agent.transform;
        goalTransform = goal;
        return FindPath();
    }

    //Initialize tileGraph and clear out path
    public void Init()
    {
        numOpenNodes = 0;
        tileWorld.Init();
        path = null;
        smoothedPath = null;
        startNode = tileGraph.WorldToTile(startTransform.position);
        goalNode = tileGraph.WorldToTile(goalTransform.position);
        priorityNodes = new List<IntPoint>();
        heuristic = new EuclideanHeuristic(goalNode);

        AddPriorityNode(startNode);

    }

    /// <summary>
    /// Performs AStar on a tile grid.
    /// </summary>
    /// <returns>
    /// True if a path was found.
    /// </returns>
    public bool FindPath()
    {
        //Prepare the tile grid, open list, and translate world positions to tile coordinates
        Init();

        //Continue searching as long as there are open nodes
        while(GetNumOpenNodes() > 0)
        {
            //Get the smallest open node using estimated total cost
            currentNode = GetSmallestOpenNode();
            TileRecord currentRecord = tileWorld.nodeArray[currentNode.x, currentNode.y];

            //If the goal is found, break out of the loop.
            if (currentNode.Equals(goalNode))
                break;

            //Get the neighboring nodes that can be reached from the currentNode
            List<IConnection<IntPoint>> neighbors;
            tileGraph.GetConnections(currentNode, out neighbors);
            foreach (IConnection<IntPoint> edge in neighbors)
            {
                IntPoint toNode = edge.GetToNode();
                TileRecord endRecord = tileWorld.nodeArray[toNode.x, toNode.y];
                float endNodeCost = currentRecord.costSoFar + edge.GetCost();
                float endNodeHeuristic = 0f;

                //Update the cost estimate.
                if (endRecord.category == NodeCategory.Closed)
                {
                    //If true, this record doesn't need to be re-opened
                    if (endRecord.costSoFar <= endNodeCost)
                        continue;
                    endNodeHeuristic = endRecord.estimatedTotalCost - endRecord.costSoFar;
                }
                else if (endRecord.category == NodeCategory.Open)
                {
                    if (endRecord.costSoFar <= endNodeCost) continue;

                    //Update the heuristic value
                    endNodeHeuristic = endRecord.estimatedTotalCost - endRecord.costSoFar;
                }
                else
                {
                    //This is an unvisited node that needs an estimate.
                    endNodeHeuristic = heuristic.Estimate(toNode);
                }

                endRecord.costSoFar = endNodeCost;
                endRecord.edge = edge;
                endRecord.estimatedTotalCost = endNodeCost + endNodeHeuristic;
                endRecord.category = NodeCategory.Open;
                tileWorld.nodeArray[toNode.x,toNode.y] = endRecord;
                AddPriorityNode(toNode);
            }

            //Close the current node
            CloseNode(currentNode);
        }
        
      
        if(currentNode.Equals(goalNode))
        {
            Debug.Log("Found Path: ");
            path = GetPath();
            return true;
        }
        else
        {
            Debug.Log("No Path Found");
            path = null;
            return false;
        }
    }
    
    public void AddPriorityNode(IntPoint node)
    {
        priorityNodes.Add(node);
        tileWorld.SetNodeCategory(node, NodeCategory.Open);
        numOpenNodes++;
    }

    public void CloseNode(IntPoint node)
    {
        priorityNodes.Remove(node);
        tileWorld.SetNodeCategory(node, NodeCategory.Closed);
        numOpenNodes--;
    }

    public int GetNumOpenNodes()
    {
        return priorityNodes.Count;
    }

    /// <summary>
    /// Finds the smallest open node using the estimated total cost.
    /// </summary>
    /// <returns>
    /// The tile coordinates of the smallest cost open node.
    /// </returns>
    public IntPoint GetSmallestOpenNode()
    {
        
        IntPoint smallestNode = priorityNodes[0];
        float smallestCost = tileWorld.GetRecordAt(smallestNode).estimatedTotalCost;
        for (int i = 1; i < GetNumOpenNodes(); i++)
        {
            float cost = tileWorld.GetRecordAt(priorityNodes[i]).estimatedTotalCost;

            if (cost < smallestCost)
            {
                smallestCost = cost;
                smallestNode = priorityNodes[i];
            }
        }

        return smallestNode;
    }

    public List<IConnection<IntPoint>> GetPathConnections()
    {
        List<IConnection<IntPoint>> result = new List<IConnection<IntPoint>>();
        IntPoint pathNode = new IntPoint(currentNode.x, currentNode.y);
        IConnection<IntPoint> edge = tileWorld.nodeArray[pathNode.x, pathNode.y].edge;
        while (!pathNode.Equals(startNode))
        {
            result.Add(edge);
            pathNode = edge.GetFromNode();
            edge = tileWorld.nodeArray[pathNode.x, pathNode.y].edge;
        }

        result.Reverse();
        return result;
    }

    /// <summary>
    /// Returns list of path points in world coordinates.
    /// </summary>
    /// <returns></returns>
    public List<Vector3> GetPath()
    {
        List<Vector3> result = new List<Vector3>();
        IntPoint pathNode = new IntPoint(currentNode.x,currentNode.y);
        IConnection<IntPoint> edge = tileWorld.nodeArray[pathNode.x, pathNode.y].edge;

        result.Add(tileGraph.TileToWorld(pathNode));
        while (!pathNode.Equals(startNode))
        {
            pathNode = edge.GetFromNode();
            edge = tileWorld.nodeArray[pathNode.x, pathNode.y].edge;
            result.Add(tileGraph.TileToWorld(pathNode));
        }
        result.Reverse();
        return result;
    }

    //Calls the path smoothing function
    public void SmoothPath()
    {
        if(path!= null)
            smoothedPath = pathSmoother.SmoothPath(path);
    }

    //Resets everything.
    public void Clear()
    {
        numOpenNodes = 0;
        tileGraph.Init();
        tileWorld.ClearNodes();
        path = null;
        smoothedPath = null;
        startNode = tileGraph.WorldToTile(startTransform.position);
        goalNode = tileGraph.WorldToTile(goalTransform.position);
        priorityNodes = new List<IntPoint>();
    }


    public void OnDrawGizmos()
    {

        if(smoothedPath != null)
        {
            foreach (Vector3 v in smoothedPath)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(v, 0.5f * tileGraph.tileSize);
            }
            if (path != null && drawUnsmoothedPath)
            {
                foreach (Vector3 v in path)
                {
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawSphere(v, 0.25f * tileGraph.tileSize);

                }
            }
        }
        else if(path != null)
        {
             foreach (Vector3 v in path)
             {
                 Gizmos.color = Color.green;
                 Gizmos.DrawSphere(v, 0.5f * tileGraph.tileSize);

             }
        }

    }

}
