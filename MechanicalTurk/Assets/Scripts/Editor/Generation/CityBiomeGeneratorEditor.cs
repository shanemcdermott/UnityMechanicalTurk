using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using Framework.Generation;
using Algorithms.City;

[CustomEditor(typeof(CityBiomeGenerator))]
public class CityBiomeGeneratorEditor : GeneratorEditor
{
    private CityBiomeGenerator cityGenerator;

    private SerializedProperty _heightMap;
    private SerializedProperty _terrain;
    private SerializedProperty _roadPainter;
    private SerializedProperty _regionGenerator;
    private SerializedProperty _gridNode;

    protected override void OnEnable()
    {
        base.OnEnable();
        cityGenerator = (CityBiomeGenerator)target;
        _heightMap = serializedObject.FindProperty("heightMap");
        _terrain = serializedObject.FindProperty("terrain");
        _roadPainter = serializedObject.FindProperty("roadPainter");
        _regionGenerator = serializedObject.FindProperty("regionGenerator");

    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        if (cityGenerator.CanGenerate())
        {
            EditorGUILayout.PropertyField(_heightMap);
            EditorGUILayout.PropertyField(_terrain);
            EditorGUILayout.PropertyField(_roadPainter);
            EditorGUILayout.PropertyField(_regionGenerator);
        }
        else
        {
            ShowMissingGenRequirements();
        }
        serializedObject.ApplyModifiedProperties();
    }


    public static CityBiomeGenerator CreateRecursiveDetailGenerator(GenerationController controller)
    {
        GameObject cityGenObj = new GameObject("City Generator");
        cityGenObj.transform.SetParent(controller.gameObject.transform);
        CityBiomeGenerator cityGen = cityGenObj.AddComponent<CityBiomeGenerator>();
        controller.cityGenerator = cityGen;
        cityGen.heightMap = controller.heightMap;
        cityGen.terrain = controller.terrainGenerator.terrain;
        cityGen.roadPainter = cityGenObj.AddComponent<RoadPainter>();
        cityGen.roadPainter.terrainData = cityGen.terrain.terrainData;
        return cityGen;
    }

    public override void ShowMissingGenRequirements()
    {
        EditorGUILayout.LabelField("Missing City Requirements", errorStyle);
        if (cityGenerator.heightMap == null)
        {
            EditorGUILayout.PropertyField(_heightMap);
        }
        if (cityGenerator.terrain == null)
        {
            EditorGUILayout.PropertyField(_terrain);
        }
        else if(cityGenerator.terrain.terrainData == null)
        {
            cityGenerator.terrain.terrainData = (TerrainData)EditorGUILayout.ObjectField("Terrain Data", cityGenerator.terrain.terrainData, typeof(TerrainData), true);
        }
        if (cityGenerator.roadPainter == null)
        {
            EditorGUILayout.PropertyField(_roadPainter);
        }
        if (cityGenerator.regionGenerator == null)
        {
            EditorGUILayout.PropertyField(_regionGenerator);
        }
    }
}
