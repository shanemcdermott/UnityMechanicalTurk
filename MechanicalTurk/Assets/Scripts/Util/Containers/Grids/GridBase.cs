using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Based off of http://www-cs-students.stanford.edu/~amitp/game-programming/grids/#relationships
//Shane McDermott 2018


public class GridBase : MonoBehaviour
{
    protected List<Vector3> vertices;
    protected List<IntPoint> edges; //Line segment connecting two vertices
    protected List<List<IntPoint>> faces; //2-D Surface bounded by edges



    public void OnDrawGizmos()
    {
        foreach(IntPoint ip in edges)
        {
            Gizmos.DrawLine(vertices[ip.x], vertices[ip.y]);
        }
    }
}
