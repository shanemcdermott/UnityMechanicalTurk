using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameNode : MonoBehaviour
{
    //Level of detail prefabs. Each should have a GameNode Component;
    public GameObject[] prefabOptions;
    public Terrain terrain;
    protected Node node;

    public void SetNode(Node node)
    {
        this.node = node;
        transform.localPosition = node.GetPosition();
//        Vector3 pos = node.GetPosition();
//       transform.position = new Vector3(pos.x, 0, pos.y);
    }

    public void SpawnBuildings()
    {
        Debug.Log("Spawning Buildings");
        GameObject go = GameObject.Instantiate(GetRandomPrefab(),transform);
        go.transform.position = transform.position;
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
