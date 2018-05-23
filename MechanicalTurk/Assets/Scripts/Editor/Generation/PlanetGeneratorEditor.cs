using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using Algorithms.Nature;

[CustomEditor(typeof(PlanetGenerator))]
public class PlanetGeneratorEditor : GeneratorEditor {

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if(GUILayout.Button("Generate"))
        {
            PlanetGenerator pGen = (PlanetGenerator)target;
            pGen.Generate();
        }
    }

    public override void ShowMissingGenRequirements()
    {
        
    }
}
