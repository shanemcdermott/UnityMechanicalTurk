using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using Framework.Generation;

[CustomEditor(typeof(GenerationSequence))]
public class GenerationSequenceEditor : GeneratorEditor
{
    private GenerationSequence _genSequence;


    protected override void OnEnable()
    {
        base.OnEnable();
        _genSequence = (GenerationSequence)target;

    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
    }

    public override void ShowMissingGenRequirements()
    {
        
    }
}
