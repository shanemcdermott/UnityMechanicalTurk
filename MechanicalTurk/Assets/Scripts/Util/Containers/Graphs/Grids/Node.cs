using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Node : IHasConnections<Node>
{
    protected Vector3 position;
    protected List<Node> connections = new List<Node>();

    public float x
    {
        get { return position.x;}
        set { position.x = value;}
    }

    public float y
    {
        get { return position.y; }
        set { position.y = value; }
    }

    public float z
    {
        get { return position.z; }
        set { position.z = value; }
    }

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

    public static void FindOrAdd(ref Dictionary<Vector2Int, bool> points, Vector2Int key, bool value)
    {
        if(!points.ContainsKey(key))
        {
            points.Add(key, value);
        }
    }

    public void GetConnectionLines(ref List<Vector2Int> connectionPoints)
    {
        foreach (Node c in connections)
        {
            if (c != null)
            {
                GetConnectionLine(ref connectionPoints, position, c.position);
            }
        }
    }

    public void GetVerticalConnections(ref Dictionary<Vector2Int, bool> connectionPoints)
    {
        foreach (Node c in connections)
        {
            if (c != null && c.z == z)
            {
                GetConnectionLine(ref connectionPoints, position, c.position);
            }
        }
    }

    public void GetHorizontalConnections(ref Dictionary<Vector2Int, bool> connectionPoints)
    {
        foreach (Node c in connections)
        {
            if (c != null && c.x == x)
            {
                GetConnectionLine(ref connectionPoints, position, c.position);
            }
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

    public static void GetConnectionLine(ref Dictionary<Vector2Int, bool> connectionLines, Node from, Node to)
    {
        GetConnectionLine(ref connectionLines, from.GetPosition(), to.GetPosition());
    }

    public static void GetConnectionLine(ref List<Vector2Int> connectionLine, Vector3 node, Vector3 connection)
    {
        for (int x = (int)node.x; x <= (int)connection.x; x++)
        {
            for (int z = (int)node.z; z <= (int)connection.z; z++)
            {
                Vector2Int v = new Vector2Int(x, z);
                if (!connectionLine.Contains(v))
                {
                    connectionLine.Add(v);
                }
            }

            for (int z = (int)node.z; z > (int)connection.z; z--)
            {
                Vector2Int v = new Vector2Int(x, z);
                if (!connectionLine.Contains(v))
                {
                    connectionLine.Add(v);
                }
            }

        }


        for (int x = (int)node.x; x > (int)connection.x; x--)
        {
            for (int z = (int)node.z; z <= (int)connection.z; z++)
            {
                Vector2Int v = new Vector2Int(x, z);
                if (!connectionLine.Contains(v))
                {
                    connectionLine.Add(v);
                }
            }

            for (int z = (int)node.z; z > (int)connection.z; z--)
            {
                Vector2Int v = new Vector2Int(x, z);
                if (!connectionLine.Contains(v))
                {
                    connectionLine.Add(v);
                }
            }

        }

    }

    public static void  GetConnectionLine(ref Dictionary<Vector2Int, bool> connectionLine, Vector3 node, Vector3 connection)
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
        return Split(A, B, 0.5f);
    }

    public static Node Split(Node A, Node B, float weight)
    {
        Node res = new Node(Vector3.Lerp(A.position, B.position, weight));
        if (A.IsConnectedTo(B))
        {
            A.RemoveConnection(B);
            res.AddConnection(A);
            res.AddConnection(B);
        }
        return res;
    }
}
