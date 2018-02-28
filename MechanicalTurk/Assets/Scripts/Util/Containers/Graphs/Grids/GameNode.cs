using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameNode : MonoBehaviour
{
    //Level of detail prefabs. Each should have a GameNode Component;
    public GameObject[] prefabOptions;
    public Vector3 spawnSpace = new Vector3(25, 0, 25);
    public Vector3 minLocation = new Vector3(-50,0, 50);
    public Vector3 maxLocation = new Vector3(-50,0, 50);

    protected Node node;

    public void SetNode(Node node)
    {
        this.node = node;
        transform.position = node.GetPosition();
    }

	// Use this for initialization
	void Start () {
		
	}
	
    public void SpawnBuildings()
    {
      for(float x = minLocation.x; x < maxLocation.x; x+=spawnSpace.x)
        {
            for(float z = minLocation.z; z < maxLocation.z; z+=spawnSpace.z)
            {
                GameObject go = GameObject.Instantiate(GetRandomPrefab());
                go.transform.SetParent(transform);
                go.transform.localPosition = new Vector3(x,0,z);
            }
        }
    }

    public GameObject GetRandomPrefab()
    {
        return prefabOptions[Random.Range(0, prefabOptions.Length)];
    }

	// Update is called once per frame
	void Update ()
    {
        node.DrawConnections();
	}
}
