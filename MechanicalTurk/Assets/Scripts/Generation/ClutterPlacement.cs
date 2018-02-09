using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Shane McDermott 2018
public class ClutterPlacement : MonoBehaviour
{

    //Prefabs
    public GameObject[] clutterPrefabs;

    //PlacementAlgorithm
    public PointGenerator pointGenerator;

    public List<GameObject> spawnedClutter;

    public void placeClutter()
    {
        List<Vector2> points;
        pointGenerator.Generate(out points);
        foreach (Vector2 v in points)
        {
            Vector3 v3 = new Vector3(v.x, 0, v.y) + transform.position;
            GameObject go = (GameObject)Instantiate(GetRandomClutter(), v3, transform.rotation);
            if(go)
            {
                go.transform.SetParent(transform);
                spawnedClutter.Add(go);
            }
        }
    }

    public void clearClutter()
    {
        foreach(GameObject go in spawnedClutter)
        {
            if (go != null)
                DestroyImmediate(go);
        }
        spawnedClutter.Clear();
    }

    public GameObject GetRandomClutter()
    {
        return clutterPrefabs[Random.Range(0, clutterPrefabs.Length)];
    }

}
