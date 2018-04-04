using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGraph<T>
{
    /// <summary>
    /// Returns an array of connections (implementing IConnection)
    /// outgoing from the given node.
    /// </summary>
    /// <param name="fromNode">The node whose connections to return.</param>
    /// <param name="connections">The connections to return.</param>
    void GetConnections(T fromNode, out List<IConnection<T>> connections);
}

public interface IHasConnections<T>
{
    /// <summary>
    /// Returns an array of connections (implementing IConnection)
    /// outgoing from self
    /// </summary>
    /// <param name="connections">The connections to return.</param>
    void GetConnections(out List<IConnection<T>> connections);

    //returns true if this has a connection to other
    bool IsConnectedTo(T other);
}

//Connection between nodes of type T
public interface IConnection<T>
{
    /// <summary>
    /// Returns the non-negative cost of the connection
    /// </summary>
    /// <returns></returns>
    float GetCost();

    /// <summary>
    /// Returns the id of the node that this connection from.
    /// </summary>
    /// <returns></returns>
    T GetFromNode();

    /// <summary>
    /// Returns the id of the node that this connection leads to.
    /// </summary>
    /// <returns></returns>
    T GetToNode();

    bool HasNode(T t);
}

public enum NodeCategory
{
    Unvisited,
    Open,
    Closed
}


public struct IntPoint
{
    public int x;
    public int y;

    public IntPoint(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public IntPoint(float x, float y)
    {
        this.x = Mathf.FloorToInt(x);
        this.y = Mathf.FloorToInt(y);
    }

    public IntPoint(Vector2 v)
    {
        this.x = Mathf.FloorToInt(v.x);
        this.y = Mathf.FloorToInt(v.y);
    }

    public static IntPoint Zero()
    {
        return new IntPoint(0, 0);
    }

    public static IntPoint Add(IntPoint a, IntPoint b)
    {
        return IntPoint.Add(a, b.x, b.y);
    }

    public static IntPoint Add(IntPoint a, int x, int y)
    {
        return new IntPoint(a.x + x, a.y + y);
    }

	public static IntPoint Sub(IntPoint a, int x, int y)
	{
		return Add (a, -x, -y);
	}

	public static IntPoint Sub(IntPoint a, IntPoint b)
	{
		return Sub (a, b.x, b.y);
	}

    public static float Distance(IntPoint a, IntPoint b)
    {
        return Vector2.Distance(a.ToVector(), b.ToVector());
    }

    public Vector2 ToVector()
    {
        return new Vector2(x, y);
    }

    public override string ToString()
    {
        return "(" + x + "," + y + ")";
    }

    /*public override bool Equals(object obj)
    {
        if (!(obj is IntPoint))
            return false;

        IntPoint otherPoint = (IntPoint)obj;
        // compare elements here
        return x == otherPoint.x && y == otherPoint.y;

    }*/

    public bool GreaterThan(IntPoint p)
    {
        return x > p.x && y > p.y;
    }

    public bool LessThan(IntPoint p)
    {
        return x < p.x && y < p.y;
    }
}

public struct TileRecord
{
    public IConnection<IntPoint> edge;
    public float costSoFar;
    public float estimatedTotalCost;
    public NodeCategory category;
}