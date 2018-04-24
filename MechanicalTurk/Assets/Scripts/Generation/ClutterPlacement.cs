using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;



//Shane McDermott 2018
public class ClutterPlacement : GenerationAlgorithm
{

    //Prefabs
    public ObjectSpawnParams[] clutterPrefabs;

    //PlacementAlgorithm
    public PointGenerator pointGenerator;

    public List<GameObject> spawnedClutter;

    //Terrain object
    public Terrain terrain;
    public Transform clutterParent;

    public override void Generate()
    {
        List<Vector2> points;
        pointGenerator.Generate(out points);
        foreach (Vector2 v in points)
        {
            Vector3 v3 = new Vector3(v.x, 0f, v.y) + clutterParent.position;
            v3.y = terrain.SampleHeight(v3);
            GameObject prefab = GetRandomClutterPrefab(GetSteepnessAt(v3));
            if(prefab != null)
            {
                GameObject go = (GameObject)Instantiate(prefab, v3, clutterParent.rotation);
                if (go)
                {
                    go.transform.SetParent(clutterParent);
                    spawnedClutter.Add(go);
                }
            }
        }
    }



    protected float GetSteepnessAt(Vector3 position)
    {
        return Mathf.Abs(terrain.terrainData.GetSteepness(position.x, position.z));
    }

    public override void Clean()
    {
        foreach(GameObject go in spawnedClutter)
        {
            if (go != null)
                DestroyImmediate(go);
        }
        spawnedClutter.Clear();
    }

    public GameObject GetRandomClutterPrefab(float steepness)
    {
        int i = GetRandomClutterIndex(steepness);
        if(i >=0)
        {
            return clutterPrefabs[i].randomObject;
        }
        return null;
    }

    public int GetRandomClutterIndex(float steepness)
    {
        for(int i = 0; i < clutterPrefabs.Length; i++)
        {
            if(steepness <= clutterPrefabs[i].maxSteepness)
            {
                return Random.Range(i, clutterPrefabs.Length);
            }
        }
        return -1;
    }

    public override bool CanGenerate()
    {
        return pointGenerator != null && clutterPrefabs.Length > 0 && terrain != null;
    }

    public override void Setup()
    {
        if(pointGenerator == null)
        {
            pointGenerator = GetComponent<PointGenerator>();
        }
        if(terrain == null)
        {
            terrain = GetComponent<Terrain>();
        }
        Clean();
        clutterPrefabs.OrderBy(i => i.maxSteepness).ThenBy(i => i.minDimensions.x).ThenBy(i => i.minDimensions.y);
    }

}
