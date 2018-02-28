using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Node : IHasConnections<Node>
{
    protected Vector3 position;
    protected List<Node> connections =new List<Node>();

    public Node()
    {
        position = new Vector3();
    }

    public Node(Vector2 position2)
    {
        position = new Vector3(position2.x, position2.y);
    }

    public Node(Vector3 position)
    {
        this.position = position;
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

    public void SetPosition(Vector3 position)
    {
        this.position = position;
    }

    public Vector3 GetPosition()
    {
        return position;
    }

    public void DrawConnections()
    {
        Vector3 pNorm = position.normalized;
        Gizmos.color = new Color(pNorm.x,pNorm.y, pNorm.z);
        foreach (Node c in connections)
        {
            if (c != null)
            {
                Gizmos.DrawLine(position, c.position);
            }
        }
    }

}
