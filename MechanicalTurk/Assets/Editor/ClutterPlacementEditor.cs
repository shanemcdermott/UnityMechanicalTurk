using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//Shane McDermott 2018

[CustomEditor(typeof(ClutterPlacement))]
public class ClutterPlacementEditor : Editor
{
    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        DrawDefaultInspector();
        ClutterPlacement clutter = (ClutterPlacement)target;
        if(GUILayout.Button("Place Clutter"))
        {
            clutter.Generate();
        }
        if(GUILayout.Button("Clear Clutter"))
        {
            clutter.Clean();
        }
    }
}
