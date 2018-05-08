using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Framework.Generation;
using Algorithms.Noise;

public abstract class GeneratorEditor : Editor
{
    protected GUIStyle errorStyle;

    protected virtual void OnEnable()
    {
        errorStyle = new GUIStyle();
        errorStyle.normal.textColor = Color.red;
    }

    public abstract void ShowMissingGenRequirements();

    public NoiseGenerator  NoiseGeneratorField(string fieldName, NoiseGenerator currentNoiseGen, GameObject parentObject)
    {
        NoiseGenerator gen = (NoiseGenerator)EditorGUILayout.ObjectField(fieldName, currentNoiseGen, typeof(NoiseGenerator), true);
        if (gen == null)
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Simplex"))
            {
                gen = parentObject.AddComponent<SimplexNoise>();
            }
            if (GUILayout.Button("Perlin Octaves"))
            {
                gen = parentObject.AddComponent<PerlinOctaves>();
            }
            EditorGUILayout.EndHorizontal();
        }
        return gen;
    }
}
