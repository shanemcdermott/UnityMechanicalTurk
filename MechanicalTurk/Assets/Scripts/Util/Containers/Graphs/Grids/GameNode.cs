using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameNode : MonoBehaviour
{
    //Level of detail prefabs. Each should have a GameNode Component;
    public GameObject[] prefabOptions;

    protected Node node;
    protected Terrain terrain;

    public void SetNode(Node node)
    {
        this.node = node;
        Vector3 pos = node.GetPosition();
        transform.position = new Vector3(pos.x, 0, pos.y);
    }

    public void SetTerrain(ref Terrain terrain)
    {
        this.terrain = terrain;
    }

    public void SpawnBuildings()
    {
        Debug.Log("Spawning Buildings");
        GameObject go = GameObject.Instantiate(GetRandomPrefab(),transform);
        float y = terrain.SampleHeight(transform.position);
        go.transform.position = new Vector3(transform.position.x, y, transform.position.z);
        /*

        float x = Random.value;
        float z = Random.value;
        Vector3 worldPos = transform.position + new Vector3(x,0,z);
        if(terrain)
        {
            float y = terrain.SampleHeight(worldPos);
            worldPos.y = worldPos.z;
            worldPos.z = y;
        }
        go.transform.position = worldPos;
          */
    }

    public GameObject GetRandomPrefab()
    {
        return prefabOptions[Random.Range(0, prefabOptions.Length)];
    }

}
