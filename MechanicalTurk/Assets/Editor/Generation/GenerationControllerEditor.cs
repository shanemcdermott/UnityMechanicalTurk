﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
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
        if(GUILayout.Button("Clear Buildings"))
        {
            while(controller.cityGenerator.transform.childCount > 0)
            {
                Transform child = controller.cityGenerator.transform.GetChild(0);
                DestroyImmediate(child.gameObject);
            }
        }

        /*
        if (GUILayout.Button("Save Heightmap"))
        {
            Texture2D tex = controller.terrainGenerator.biomeTexture;
            string fileName = Application.persistentDataPath + "/" + tex.name + ".png";
            File.WriteAllBytes(fileName, tex.EncodeToPNG());
            Debug.Log("Saved to " + fileName);
        }
        */
    }
}
