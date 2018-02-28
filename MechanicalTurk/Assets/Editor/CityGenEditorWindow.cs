using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CityGenEditorWindow : EditorWindow
{

    [MenuItem("Window/Feature Comparison")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(CityGenEditorWindow));
    }

    private void OnGUI()
    {
        GUILayout.Label("Coming soon!");
    }

}
