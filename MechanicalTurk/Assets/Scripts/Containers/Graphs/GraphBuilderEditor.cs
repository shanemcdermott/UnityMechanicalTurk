using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GraphBuilder))]
public class GraphBuilderEditor : Editor
{

    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        DrawDefaultInspector();
        GraphBuilder _target = (GraphBuilder)target;

        if (GUILayout.Button("Initialize"))
        {
            _target.Init();
        }

        if (GUILayout.Button("Next Point"))
        {
            _target.drawTriangles = false;
            _target.drawPoints = false;
            _target.NextPoint();
        }

        if (GUILayout.Button("Fill"))
        {
            _target.FillPoints();
        }
    }

    /*
    public void OnSceneGUI()
    {
        GraphBuilder _target = (GraphBuilder)target;
        Vector2[] points = _target.delTri.GetPoints();
        foreach (Vector2 p in points)
        {
            Handles.Label(new Vector3(p.x, p.y, 0f), p.ToString());
        }
    }
    */
}
