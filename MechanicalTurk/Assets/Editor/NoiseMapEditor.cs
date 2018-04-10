using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(NoiseMap))]
public class NoiseMapEditor : Editor
{

    public int tab;
    public string[] tabs = new string[] { "Power of 2", "Square", "Rectangle" };

    public override void OnInspectorGUI()
    {
       
        NoiseMap noiseMap = (NoiseMap)target;

        tab = GUILayout.Toolbar(tab, tabs);
        switch (tab)
        {
            case 0:
                int currentPower = Mathf.RoundToInt(Mathf.Log(noiseMap.Width, 2));
                int power = EditorGUILayout.IntField("Exponent", currentPower);
                if(currentPower != power)
                {
                    int res = Mathf.RoundToInt(Mathf.Pow(2, power));
                    noiseMap.Dimensions = new Vector2Int(res,res);
                }
                break;
            case 1:
                noiseMap.Width = EditorGUILayout.IntField("Size", noiseMap.GetWidth());
                noiseMap.Height = noiseMap.Width;
                break;
            case 2:
                noiseMap.Width = EditorGUILayout.IntField("Width", noiseMap.GetWidth());
                noiseMap.Height = EditorGUILayout.IntField("Height", noiseMap.GetHeight());
                break;
        }
    }
}
