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
    private SerializedProperty _gridNode;

    protected override void OnEnable()
    {
        base.OnEnable();
        cityGenerator = (CityBiomeGenerator)target;
        _heightMap = serializedObject.FindProperty("heightMap");
        _terrain = serializedObject.FindProperty("terrain");
        _roadPainter = serializedObject.FindProperty("roadPainter");

    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        if (cityGenerator.CanGenerate())
        {
            EditorGUILayout.PropertyField(_heightMap);
            EditorGUILayout.PropertyField(_roadPainter);
            ShowRegionGenFields();
        }
        else
        {
            ShowMissingGenRequirements();
        }
        serializedObject.ApplyModifiedProperties();
    }


    public static CityBiomeGenerator CreateRecursiveDetailGenerator(GenerationController controller)
    {
        CityBiomeGenerator cityGen = CreateRecursiveDetailGenerator();
        cityGen.gameObject.transform.SetParent(controller.gameObject.transform);
        controller.cityGenerator = cityGen;
        cityGen.heightMap = controller.heightMap;
        if(controller.terrainGenerator)
            cityGen.terrain = controller.terrainGenerator.terrain;

        return cityGen;
    }

    public static CityBiomeGenerator CreateRecursiveDetailGenerator()
    {
        GameObject cityGenObj = new GameObject("City Generator");
        CityBiomeGenerator cityGen = cityGenObj.AddComponent<CityBiomeGenerator>();
        cityGen.roadPainter = cityGenObj.AddComponent<RoadPainter>();
        cityGen.regionGenerator = cityGenObj.AddComponent<CityBlockGenerator>();
        return cityGen;
    }

    public override void ShowMissingGenRequirements()
    {
        EditorGUILayout.LabelField("Missing Requirements for City Generation", errorStyle);
        if (cityGenerator.heightMap == null)
        {
            EditorGUILayout.PropertyField(_heightMap);
        }
        if (cityGenerator.terrain == null)
        {
            cityGenerator.terrain = (Terrain)EditorGUILayout.ObjectField("Terrain", cityGenerator.terrain, typeof(Terrain), true);
        }
        else if(cityGenerator.terrain.terrainData == null)
        {
            cityGenerator.terrain.terrainData = (TerrainData)EditorGUILayout.ObjectField("Terrain Data", cityGenerator.terrain.terrainData, typeof(TerrainData), true);
        }
        if (cityGenerator.roadPainter == null)
        {
            EditorGUILayout.PropertyField(_roadPainter);
        }
        ShowRegionGenFields();
    }

    public virtual void ShowRegionGenFields()
    {
        if (cityGenerator.regionGenerator == null)
        {
            cityGenerator.regionGenerator = (BlockGenerator)EditorGUILayout.ObjectField("Region Generator", cityGenerator.regionGenerator, typeof(BlockGenerator), true);
        }
        else if (!cityGenerator.regionGenerator.CanGenerate())
        {
            CityBlockGeneratorEditor regEditor = (CityBlockGeneratorEditor)CreateEditor(cityGenerator.regionGenerator);
            regEditor.ShowMissingGenRequirements();
        }
    }


}
