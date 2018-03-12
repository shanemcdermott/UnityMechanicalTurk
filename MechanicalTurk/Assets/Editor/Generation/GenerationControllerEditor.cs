using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GenerationController))]
public class GenerationControllerEditor : Editor
{

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        GenerationController controller = (GenerationController)target;
        if(GUILayout.Button("Generate"))
        {
            controller.SetupAndGenerate();
        }
    }
}
