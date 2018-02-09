using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridEdge : MonoBehaviour,  IConnection<Vector3>
{
    public Vector3 A;
    public Vector3 B;

    public GridEdge(Vector3 A, Vector3 B)
    {
        this.A = A;
        this.B = B;
    }

    public float GetCost()
    {
        return Vector3.Distance(A, B);
    }

    public Vector3 GetFromNode()
    {
        return A;
    }

    public Vector3 GetToNode()
    {
        return B;
    }
}
