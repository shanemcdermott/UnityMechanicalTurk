using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Node : IHasConnections<Node>
{
    protected Vector3 position;
    protected List<Node> connections = new List<Node>();

    public Node()
    {
        position = new Vector3();
    }

    public Node(Vector2 position2)
    {
        position = new Vector3(position2.x, 0, position2.y);
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

    public Vector2 GetPositionXZ()
    {
        return new Vector2(position.x, position.z);
    }

    public Vector3 GetPosition()
    {
        return position;
    }

    public virtual void DrawConnections()
    {
        Vector3 pNorm = position.normalized;
       
        Color color = new Color(pNorm.x,pNorm.y, pNorm.z);
        foreach (Node c in connections)
        {
            if (c != null)
            {
                Debug.DrawLine(position, c.position, color);
            }
        }
    }

    public List<Vector2Int> GiveConnectionLines()
    {
        List<Vector2Int> connectionPoints = new List<Vector2Int>();
        
        foreach (Node c in connections)
        {
            if (c != null)
            {
                connectionPoints.AddRange(GetConnectionLine(position, c.position));
            }
        }

        return connectionPoints;
    }

    public List<Vector2Int> GetConnectionLine(Vector3 node, Vector3 connection)
    {
        List<Vector2Int> connectionLine = new List<Vector2Int>();

        Vector3 difference = connection - node;

        if(difference.x > 0)
        {
            for(int i = (int)node.x; i <= (int)connection.x; i++)
            {
                connectionLine.Add(new Vector2Int(i,(int)node.z));
            }
        }
        else if(difference.x < 0)
        {
            for (int i = (int)connection.x; i <= (int)node.x; i++)
            {
                connectionLine.Add(new Vector2Int(i, (int)connection.z));
            }
        }

        if (difference.z > 0)
        {
            for (int i = (int)node.z; i <= (int)connection.z; i++)
            {
                connectionLine.Add(new Vector2Int((int)node.x, i));
            }
        }
        else if(difference.z < 0){
            for (int i = (int)connection.z; i <= (int)node.z; i++)
            {
                connectionLine.Add(new Vector2Int((int)connection.x, i));
            }
        }

        return connectionLine;
    }

    public static Node Split(Node A, Node B)
    {
        Node res = new Node(MathOps.Midpoint(A.position, B.position));
        if(A.IsConnectedTo(B))
        {
            A.RemoveConnection(B);
            res.AddConnection(A);
            res.AddConnection(B);
        }
        return res;
    }
}
