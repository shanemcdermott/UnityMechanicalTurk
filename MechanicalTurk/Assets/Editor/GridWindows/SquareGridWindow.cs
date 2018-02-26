﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SquareGridWindow : GridWindow
{

    SquareGridParams gridParams;

    public SquareGridWindow()
    {
        gridParams = new SquareGridParams();
        gridParams.Dimensions = new Vector2(100, 100);
        gridParams.FacesPerSide = new Vector2Int(10, 10);
    }

    public override void ShowParams()
    {
        gridParams.Dimensions = EditorGUILayout.Vector2Field("Dimensions", gridParams.Dimensions);
        gridParams.FacesPerSide = EditorGUILayout.Vector2IntField("Faces Per Size", gridParams.FacesPerSide);

        GUI.enabled = false;
        EditorGUILayout.Vector2Field("Face Dimensions", gridParams.GetFaceDimensions());
        GUI.enabled = true;
    }

    public override void CreateGrid(out PolyGrid grid)
    {
        if (!GridFactory.CreateSquareGrid(gridParams, out grid))
        {
            Debug.Log("Failed to create Grid!");
        }
    }
}
