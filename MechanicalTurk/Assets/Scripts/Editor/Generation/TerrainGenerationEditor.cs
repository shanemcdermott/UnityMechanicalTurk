using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

using Framework.Generation;
using Framework.Collections;
using Algorithms.Noise;

[CustomEditor(typeof(TerrainGenerator))]
public class TerrainGenerationEditor : GeneratorEditor
{

    TerrainGenerator _generator;
    private TerrainCollider _terrainCollider;


    private SerializedProperty _heightMapGenerator;
    private SerializedProperty _biomeGenerator;
    private SerializedProperty _heightScale;
    private SerializedProperty _terrain;


    protected override void OnEnable()
    {
        base.OnEnable();
         _generator = (TerrainGenerator)target;
        _terrainCollider = _generator.GetComponentInChildren<TerrainCollider>();

        _heightMapGenerator = serializedObject.FindProperty("heightMapGenerator");
        _biomeGenerator = serializedObject.FindProperty("biomeGenerator");
        _heightScale = serializedObject.FindProperty("heightScale");
        _terrain = serializedObject.FindProperty("terrain");
        _generator.Setup();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        if (_generator.CanGenerate())
        {
            EditorGUILayout.PropertyField(_heightMapGenerator);
            EditorGUILayout.PropertyField(_heightScale);
            EditorGUILayout.PropertyField(_biomeGenerator);
            _generator.biomeTexture = (Texture2D)EditorGUILayout.ObjectField("Biome Texture", _generator.biomeTexture, typeof(Texture2D), true);
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

    public virtual void ShowTerrainDataField()
    {
        if (_generator.terrain == null)
        {
            EditorGUILayout.PropertyField(_terrain);
        }
        else if (_generator.terrain.terrainData == null)
        {
            _terrainCollider.terrainData = _generator.terrain.terrainData = (TerrainData)EditorGUILayout.ObjectField("Terrain Data", _generator.terrain.terrainData, typeof(TerrainData), true);
        }
    }

    public virtual void ShowHeightMapField()
    {
        NoiseMap noiseMap = _generator.heightMap;
        if (noiseMap == null)
        {
            noiseMap = (NoiseMap)EditorGUILayout.ObjectField("Height Map", noiseMap, typeof(NoiseMap), true);
        }
        _generator.heightMap = noiseMap;
    }

    public override void ShowMissingGenRequirements()
    {
        EditorGUILayout.LabelField("Missing Requirements for Terrain Generation", errorStyle);
        if(_generator.heightMap == null)
        {
            ShowHeightMapField();
        }

        if(_generator.heightMapGenerator == null)
        {
            NoiseGenerator noiseGen = NoiseGeneratorField("HeightMap Generator", null, _generator.gameObject);
            if(noiseGen!= null)
            {
                noiseGen.noiseMap = _generator.heightMap;
                _generator.heightMapGenerator = noiseGen;
            }
        }

        if(_generator.biomeGenerator == null)
        {
            EditorGUILayout.PropertyField(_biomeGenerator);
        }
        if(_generator.biomeTexture == null)
        {
            _generator.biomeTexture = (Texture2D)EditorGUILayout.ObjectField("Biome Texture", _generator.biomeTexture, typeof(Texture2D), true);
        }

        ShowTerrainDataField();

    }

    [MenuItem("GameObject/Generation/Terrain")]
    public static TerrainGenerator CreateTerrainGenerator()
    {
        GameObject terObj = new GameObject("Terrain Generator");
        TerrainGenerator terGen = terObj.AddComponent<TerrainGenerator>();
        BiomeGenerator biomeGen = terObj.AddComponent<BiomeGenerator>();
        NoiseMap noiseMap = terObj.AddComponent<NoiseMap>();


        terGen.heightMap = noiseMap;
        biomeGen.heightMap = noiseMap;
        terGen.terrain = terObj.AddComponent<Terrain>();
        terObj.AddComponent<TerrainCollider>();

        Selection.activeGameObject = terObj;
        return terGen;
    }
}
