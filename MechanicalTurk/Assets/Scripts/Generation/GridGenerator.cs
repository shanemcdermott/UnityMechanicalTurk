using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    public PolyGrid polyGrid;
    public ObjectPicker objectSelector;

    public void Start()
    {
        if (polyGrid.NumFaces() == 0)
        {
            Debug.Log("populating grid");
            GridFactory.PopulateSquareGrid(ref polyGrid);
            /*
            foreach (Node node in polyGrid.GetFaces())
            {
                GameObject go = GameObject.Instantiate(LOD_0_Prefabs[Random.Range(0, LOD_0_Prefabs.Length)]);
                go.transform.SetParent(transform);
                GameNode gn = go.GetComponent<GameNode>();
                gn.SetNode(node);
                gn.SpawnBuildings();
            }
            */
        }
    }

}
