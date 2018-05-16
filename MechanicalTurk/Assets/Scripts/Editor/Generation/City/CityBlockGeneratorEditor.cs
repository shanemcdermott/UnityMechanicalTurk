using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using Framework.Generation;
using Algorithms.City;

[CustomEditor(typeof(CityBlockGenerator))]
public class CityBlockGeneratorEditor : GeneratorEditor
{

    private CityBlockGenerator _controller;

    private SerializedProperty _prefabs;

    protected override void OnEnable()
    {
        base.OnEnable();
        _controller = (CityBlockGenerator)target;
        _prefabs = serializedObject.FindProperty("blockPrefabs");

    }

    public override void ShowMissingGenRequirements()
    {
        EditorGUILayout.LabelField("Missing Requirements", errorStyle);
        EditorGUILayout.PropertyField(_prefabs);
    }
}
