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
    }

    public GameObject GetRandomPrefab()
    {
        return prefabOptions[Random.Range(0, prefabOptions.Length)];
    }

}
