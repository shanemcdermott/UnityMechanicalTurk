using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//Shane McDermott 2018

[CustomEditor(typeof(DelaunayTriangulation))]
public class DelaunayTriangulationEditor : Editor
{
    /*
    private void OnSceneGUI()
    {
        DelaunayTriangulation _target = (DelaunayTriangulation)target;
        Vector2[] points = _target.GetPoints();
        foreach(Vector2 p in points)
        {
            Handles.Label(new Vector3(p.x, p.y, 0f), p.ToString());
        }
    }
    */
}
