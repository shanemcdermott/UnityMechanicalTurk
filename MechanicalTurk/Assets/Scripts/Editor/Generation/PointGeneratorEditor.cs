using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using Framework.Generation;
//Shane McDermott 2018

[CustomEditor(typeof(PointGenerator))]
public class PointGeneratorEditor : Editor
{

    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        DrawDefaultInspector();
        PointGenerator clutter = (PointGenerator)target;
        if (GUILayout.Button("Initialize"))
        {
            clutter.Setup();
        }
        if (GUILayout.Button("Next Point"))
        {
            clutter.NextPoint();
        }
    }
}
