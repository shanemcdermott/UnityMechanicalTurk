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

    //Map Mesh
    public MeshFilter mapMeshFilter;

    public void placeClutter()
    {
        List<Vector2> points;
        pointGenerator.Generate(out points);
        foreach (Vector2 v in points)
        {
            float mapHeight = GetHeightAtPostion(v);
            Vector3 v3 = new Vector3(v.x, mapHeight, v.y) + transform.position;
            GameObject go = (GameObject)Instantiate(GetRandomClutter(), v3, transform.rotation);
            if(go)
            {
                go.transform.SetParent(transform);
                spawnedClutter.Add(go);
            }
        }
    }

    float GetHeightAtPostion(Vector2 v2)
    {
        Mesh mapMesh = mapMeshFilter.sharedMesh;
        Vector3[] meshVerts = mapMesh.vertices;
        

        for(int x = 0; x < meshVerts.Length; x++)
        {
            if (meshVerts[x].x - v2.x < 3 && meshVerts[x].z - v2.y < 3 && CheckSlopeForPlacement(x))
            {
                return meshVerts[x].y;
            }
        }

        return 0;
    }

    bool CheckSlopeForPlacement(int meshIndice)
    {
        Mesh mapMesh = mapMeshFilter.sharedMesh;
        Vector3[] meshNormals = mapMesh.normals;
        Vector3 normalToCheck = meshNormals[meshIndice];
        float zySlope = normalToCheck.y / normalToCheck.z;
        float xySlope = normalToCheck.y / normalToCheck.x;

        if(Mathf.Abs(zySlope) < 0.2f && Mathf.Abs(xySlope) < 0.2f)
        {
            return true;
        }
        return false;
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
