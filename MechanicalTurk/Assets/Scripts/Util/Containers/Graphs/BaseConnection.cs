using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseConnection<T> : IConnection<T>
{
    public T fromNode;
    public float cost = 1;
    public T toNode;

    public BaseConnection(T from, T to)
    {
        this.fromNode = from;
        this.toNode = to;
    }

    public float GetCost()
    {
        return cost;
    }

    public T GetFromNode()
    {
        return fromNode;
    }

    public T GetToNode()
    {
        return toNode;
    }

}
