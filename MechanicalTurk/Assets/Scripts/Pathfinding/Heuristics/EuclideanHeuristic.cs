using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EuclideanHeuristic : Heuristic<IntPoint>
{

    public EuclideanHeuristic(IntPoint goal)
    {
        this.goal = goal;
    }

    /// <summary>
    /// Returns Euclidean distance between given node and goal node.
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    public override float Estimate(IntPoint node)
    {
        return IntPoint.Distance(node, goal);
    }
}
