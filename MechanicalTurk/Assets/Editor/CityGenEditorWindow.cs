using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CityGenEditorWindow : EditorWindow
{


    [MenuItem("GameObject/City Generator")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(CityGenEditorWindow));
    }

    private void OnGUI()
    {
        GUILayout.Label("Coming soon!");
    }

}
