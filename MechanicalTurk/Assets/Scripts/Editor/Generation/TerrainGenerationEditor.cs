using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

using Framework.Generation;

[CustomEditor(typeof(TerrainGenerator))]
public class TerrainGenerationEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        TerrainGenerator generator = (TerrainGenerator)target;
        if (GUILayout.Button("Generate"))
        {
            generator.Setup();
            generator.Generate();
        }
        if (GUILayout.Button("Export Map Texture"))
        {
            Texture2D tex = generator.biomeTexture;
            string fileName = Application.persistentDataPath + "/" + tex.name + ".png";
            File.WriteAllBytes(fileName, tex.EncodeToPNG());
            Debug.Log("Saved to " + fileName);
        }
    }
}
