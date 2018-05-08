using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

using Framework.Generation;
using Algorithms.Noise;

[CustomEditor(typeof(TerrainGenerator))]
public class TerrainGenerationEditor : GeneratorEditor
{

    TerrainGenerator _generator;

    private SerializedProperty _heightMapGenerator;

    private SerializedProperty _biomeGenerator;
    private SerializedProperty _heightScale;
    private SerializedProperty _heightMap;
    private SerializedProperty _terrain;
    private SerializedProperty _biomeTexture;


    protected override void OnEnable()
    {
        base.OnEnable();
         _generator = (TerrainGenerator)target;
        _heightMapGenerator = serializedObject.FindProperty("heightMapGenerator");
        _biomeGenerator = serializedObject.FindProperty("biomeGenerator");
        _heightScale = serializedObject.FindProperty("heightScale");
        _heightMap = serializedObject.FindProperty("heightMap");
        _terrain = serializedObject.FindProperty("terrain");
        _biomeTexture = serializedObject.FindProperty("biomeTexture");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        if (_generator.CanGenerate())
        {
            EditorGUILayout.PropertyField(_heightMap);
            EditorGUILayout.PropertyField(_heightMapGenerator);
            EditorGUILayout.PropertyField(_heightScale);
            EditorGUILayout.PropertyField(_terrain);
            EditorGUILayout.PropertyField(_biomeGenerator);
            EditorGUILayout.PropertyField(_biomeTexture);
            if (GUILayout.Button("Generate"))
            {
                _generator.Setup();
                _generator.Generate();
            }
        }
        else
        {
            ShowMissingGenRequirements();
        }
        if (GUILayout.Button("Export Map Texture"))
        {
            Texture2D tex = _generator.biomeTexture;
            string fileName = Application.persistentDataPath + "/" + tex.name + ".png";
            File.WriteAllBytes(fileName, tex.EncodeToPNG());
            Debug.Log("Saved to " + fileName);
        }

        serializedObject.ApplyModifiedProperties();
    }

    public override void ShowMissingGenRequirements()
    {
        EditorGUILayout.LabelField("Missing Requirements", errorStyle);
        if(_generator.heightMap == null)
        {
            EditorGUILayout.PropertyField(_heightMap);
        }
        if(_generator.heightMapGenerator == null)
        {
            _generator.heightMapGenerator = NoiseGeneratorField("HeightMap Generator", null, _generator.gameObject);
        }
        if(_generator.biomeGenerator == null)
        {
            EditorGUILayout.PropertyField(_biomeGenerator);
        }
        if(_generator.biomeTexture == null)
        {
            EditorGUILayout.PropertyField(_biomeTexture);
        }

        if (_generator.terrain == null)
        {
            EditorGUILayout.PropertyField(_terrain);
        }
        else if(_generator.terrain.terrainData == null)
        {
            _generator.terrain.terrainData = (TerrainData)EditorGUILayout.ObjectField("Terrain Data", _generator.terrain.terrainData,typeof(TerrainData), true);
        }
    }
}
