using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameNode : MonoBehaviour
{
    protected Node node;

    public void SetNode(Node node)
    {
        this.node = node;
        transform.position = node.GetPosition();
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        node.DrawConnections();
	}
}
