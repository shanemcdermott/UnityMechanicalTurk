using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Node : MonoBehaviour, IHasConnections<Node>
{
	protected List<Node> connections =new List<Node>();

    public void Start()
    {
    }

    /// <summary>
    /// Adds NodeEdge to edges as well as to @connection's edges.
    /// </summary>
    /// <param name="connection">
    /// The 'to' Node to add a connection to.
    /// </param>

    public void AddConnection(Node connection)
    {
        if(IsNullOrThis(connection))
        {
            return;
        }
        if(!IsConnectedTo(connection))
        {
            connections.Add(connection);
            connection.AddConnection(this);
        }


    }

    public void RemoveConnection(Node connection)
    {
        if(connections.Remove(connection))
        {
            connection.RemoveConnection(this);
        }
    }

    public bool IsNullOrThis(Node c)
    {
        return c == null || c.Equals(this);
    }

    public int IndexOf(Node other)
    {
        return connections.IndexOf(other);
    }

    public bool IsConnectedTo(Node other)
    {
        return connections.Contains(other);
    }

    public int NumConnections()
    {
        return connections.Count;
    }

    public void GetConnections(out List<IConnection<Node>> connections)
    {
        connections = new List<IConnection<Node>>();
        foreach(Node n in this.connections)
        {
            connections.Add(new NodeEdge(this, n));
        }
    }

    public void OnDrawGizmos()
    {
        DrawConnections();
    }

    public void DrawConnections()
    {
        foreach (Node c in connections)
        {
            if (c != null)
            {
                Gizmos.DrawLine(transform.position, c.transform.position);
            }
        }
    }

}
