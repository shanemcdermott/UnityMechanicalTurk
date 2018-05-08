using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using Framework.Generation;
using Algorithms.City;

[CustomEditor(typeof(PerlinCityGenerator))]
public class PerlinCityGeneratorEditor : GeneratorEditor
{
    private PerlinCityGenerator cityGenerator;


    protected override void OnEnable()
    {
        base.OnEnable();
        cityGenerator = (PerlinCityGenerator)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        ShowMissingGenRequirements();
    }

    public static PerlinCityGenerator CreatePerlinCityGenerator(GenerationController controller)
    {
        GameObject cityGenObj = new GameObject("City Generator");
        cityGenObj.transform.SetParent(controller.gameObject.transform);
        PerlinCityGenerator cityGen = cityGenObj.AddComponent<PerlinCityGenerator>();
        controller.cityGenerator = cityGen;
        cityGen.heightMap = controller.heightMap;
        cityGen.terrainGenerator = controller.terrainGenerator;
        cityGen.roadPainter = cityGenObj.AddComponent<RoadPainter>();
        cityGen.roadPainter.terrainData = controller.terrainGenerator.terrain.terrainData;
        cityGen.buildingGenerator = cityGenObj.AddComponent<BuildingGenerator>();
        return cityGen;
    }

    public override void ShowMissingGenRequirements()
    {
        EditorGUILayout.LabelField("Missing City Requirements", errorStyle);
        if(cityGenerator.polyGrid == null)
        {

        }
        if(cityGenerator.buildingNoiseGenerator == null)
        {
            cityGenerator.buildingNoiseGenerator = NoiseGeneratorField("Building Noise Generator", null, cityGenerator.gameObject);
        }
    }
}
