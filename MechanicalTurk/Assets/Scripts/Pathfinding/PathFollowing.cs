using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Shane McDermott 2017
public class PathFollowing : MonoBehaviour
{


    public TileAStar pathFinder;
    public float speed = 0.5f;
    public Transform goal;

    public List<Vector3> path = new List<Vector3>();
    public int pathIndex = -1;

    public float acceptableDistance = 0.25f;

    public bool needsPath = true;
    public bool usesSmoothing = true;
    public bool shouldMove = false;

    // Use this for initialization
    void Start ()
    {
	    if(needsPath)
        {
            if(pathFinder.FindAgentPath(gameObject, goal))
            {
                path = pathFinder.GetPath();
                if (usesSmoothing)
                {
                    path = pathFinder.pathSmoother.SmoothPath(path);
                }
                
                StartPath();
            }
        }
	}
	

    public void StartPath()
    {
        pathIndex = 0;
        shouldMove = true;
    }


	// Update is called once per frame
	void Update ()
    {
        if (shouldMove)
        {
            if (Vector3.Distance(transform.root.position, path[pathIndex]) <= acceptableDistance)
            {
                pathIndex++;
                if(pathIndex>=path.Count)
                {
                    shouldMove = false;
                    return;
                }
            }
            transform.root.position = Vector3.MoveTowards(transform.root.position, path[pathIndex], Time.deltaTime * speed);
        }
	}
}
