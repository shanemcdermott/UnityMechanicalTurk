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

    public void FindOrAdd(ref Dictionary<Vector2Int, bool> points, Vector2Int key, bool value)
    {
        if(!points.ContainsKey(key))
        {
            points.Add(key, value);
        }
    }

    public void GetConnectionLines(ref Dictionary<Vector2Int, bool> connectionPoints)
    {
        foreach (Node c in connections)
        {
            if (c != null)
            {
                GetConnectionLine(ref connectionPoints, position, c.position);
            }
        }
    }

    public void GetConnectionLine(ref Dictionary<Vector2Int, bool> connectionLines, Node from, Node to)
    {
        GetConnectionLine(ref connectionLines, from.GetPosition(), to.GetPosition());
    }

    public void  GetConnectionLine(ref Dictionary<Vector2Int, bool> connectionLine, Vector3 node, Vector3 connection)
    {
        Vector3 difference = connection - node;

        if (difference.x > 0)
        {
            for (int i = (int)node.x; i <= (int)connection.x; i++)
            {
                FindOrAdd(ref connectionLine, new Vector2Int(i, (int)node.z), false);
            }
        }
        else if (difference.x < 0)
        {
            for (int i = (int)connection.x; i <= (int)node.x; i++)
            {
                FindOrAdd(ref connectionLine, new Vector2Int(i, (int)connection.z), false);
            }
        }

        if (difference.z > 0)
        {
            for (int i = (int)node.z; i <= (int)connection.z; i++)
            {
                FindOrAdd(ref connectionLine, new Vector2Int((int)node.x, i), true);
            }
        }
        else if (difference.z < 0)
        {
            for (int i = (int)connection.z; i <= (int)node.z; i++)
            {
               FindOrAdd(ref connectionLine, new Vector2Int((int)connection.x, i), true);
            }
        }
    }

    public Dictionary<Vector2Int, bool> GiveConnectionLines()
    {
        Dictionary<Vector2Int, bool> connectionPoints = new Dictionary<Vector2Int, bool>();
        
        foreach (Node c in connections)
        {
            if (c != null)
            {
                Dictionary<Vector2Int, bool> connections = GetConnectionLine(position, c.position);

                foreach (KeyValuePair<Vector2Int, bool> kvp in connections)
                {
                    bool value;
                    if (!connectionPoints.TryGetValue(kvp.Key, out value))
                    {
                        connectionPoints.Add(kvp.Key, kvp.Value);
                    }
                }
            }
        }

        return connectionPoints;
    }

    public Dictionary<Vector2Int, bool> GetConnectionLine(Vector3 node, Vector3 connection)
    {
        Dictionary<Vector2Int, bool> connectionLine = new Dictionary<Vector2Int, bool>();

        Vector3 difference = connection - node;

        if(difference.x > 0)
        {
            for(int i = (int)node.x; i <= (int)connection.x; i++)
            {
                connectionLine.Add(new Vector2Int(i,(int)node.z), false);
            }
        }
        else if(difference.x < 0)
        {
            for (int i = (int)connection.x; i <= (int)node.x; i++)
            {
                connectionLine.Add(new Vector2Int(i, (int)connection.z), false);
            }
        }

        if (difference.z > 0)
        {
            for (int i = (int)node.z; i <= (int)connection.z; i++)
            {
                connectionLine.Add(new Vector2Int((int)node.x, i), true);
            }
        }
        else if(difference.z < 0){
            for (int i = (int)connection.z; i <= (int)node.z; i++)
            {
                connectionLine.Add(new Vector2Int((int)connection.x, i), true);
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
