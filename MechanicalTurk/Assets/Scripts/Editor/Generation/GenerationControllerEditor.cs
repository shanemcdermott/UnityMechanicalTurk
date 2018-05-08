using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

using Framework.Generation;
using Framework.Collections;
using Algorithms.City;



/*Editor Inspector UI for interacting with GenerationController */
[CustomEditor(typeof(GenerationController))]
public class GenerationControllerEditor : Editor
{
    private GenerationController controller;

    private SerializedProperty _seed;
    private SerializedProperty _terrainGen;
    private SerializedProperty _cityGen;

    private int cityGenTab;
    private string[] cityGenNames = { "None", "Recursive Detail", "Noise Based", "L-System" };

    private void OnEnable()
    {
       controller = (GenerationController)target;
        _seed = serializedObject.FindProperty("Seed");
        _terrainGen = serializedObject.FindProperty("terrainGenerator");
        _cityGen = serializedObject.FindProperty("cityGenerator");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(_seed);
        EditorGUILayout.PropertyField(_terrainGen);
        if (!controller.terrainGenerator.CanGenerate())
        {
            TerrainGenerationEditor terEditor = (TerrainGenerationEditor)CreateEditor(controller.terrainGenerator);
            terEditor.ShowMissingGenRequirements();
        }
        if (controller.terrainGenerator.terrain != null)
        {
            controller.terrainGenerator.terrain.terrainData = (TerrainData)EditorGUILayout.ObjectField("Terrain Data", controller.terrainGenerator.terrain.terrainData, typeof(TerrainData), true);
        }

        EditorGUILayout.PropertyField(_cityGen);
        if (controller.cityGenerator == null)
        {
            cityGenTab = GUILayout.Toolbar(cityGenTab, cityGenNames);
            switch (cityGenTab)
            {
                case 1:
                    CityBiomeGenerator gen = CityBiomeGeneratorEditor.CreateRecursiveDetailGenerator(controller);
                    // CreateCityGenerator<CityBiomeGenerator>();
                    //gen.terrain = controller.terrainGenerator.terrain;
                    break;
                case 2:

                    PerlinCityGenerator pGen = PerlinCityGeneratorEditor.CreatePerlinCityGenerator(controller);
                    //pGen.terrainGenerator = controller.terrainGenerator;
                    break;
                case 3:
                    LSystemGeneration lGen = CreateCityGenerator<LSystemGeneration>();
                    lGen.terrainGenerator = controller.terrainGenerator;
                    break;
            }
        }
        else if (!controller.cityGenerator.CanGenerate())
        {
            GeneratorEditor cityEditor = (GeneratorEditor)CreateEditor(controller.cityGenerator);
            cityEditor.ShowMissingGenRequirements();
        }
        else if(controller.terrainGenerator.CanGenerate())
        {
            if (GUILayout.Button("Generate"))
            {
                controller.SetupAndGenerate();
            }
            if (GUILayout.Button("Clear Buildings"))
            {
                while (controller.cityGenerator.transform.childCount > 0)
                {
                    Transform child = controller.cityGenerator.transform.GetChild(0);
                    DestroyImmediate(child.gameObject);
                }
                if (controller.cityGenerator is LSystemGeneration)
                {
                    ((LSystemGeneration)controller.cityGenerator).clearBuildings();
                }
            }
        }
        serializedObject.ApplyModifiedProperties();

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

    public T CreateCityGenerator<T>() where T : CityGenerator
    {
        GameObject citObj = new GameObject("City Generator");
        citObj.transform.SetParent(controller.transform);
        T generator = citObj.AddComponent<T>();
        controller.cityGenerator = generator;
        controller.cityGenerator.heightMap = controller.heightMap;
        return generator;
    }


    [MenuItem("GameObject/Generation Controller")]
    public static GenerationController CreateGenerationController()
    {
        GameObject camObj = new GameObject("GenerationController");
        GenerationController camCon = camObj.AddComponent<GenerationController>();

        GameObject terObj = new GameObject("Terrain Generator");
        terObj.transform.SetParent(camObj.transform);
        camCon.terrainGenerator = terObj.AddComponent<TerrainGenerator>();
        camCon.terrainGenerator.terrain = terObj.AddComponent<Terrain>();
        terObj.AddComponent<TerrainCollider>();

        camCon.terrainGenerator.biomeGenerator = terObj.AddComponent<BiomeGenerator>();

        NoiseMap noiseMap = terObj.AddComponent<NoiseMap>();
        camCon.terrainGenerator.heightMap = noiseMap;
        camCon.terrainGenerator.biomeGenerator.heightMap = noiseMap;

        Selection.activeGameObject = camObj;
        return camCon;
    }
}
