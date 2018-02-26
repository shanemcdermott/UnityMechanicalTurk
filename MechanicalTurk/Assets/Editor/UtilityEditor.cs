using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class UtilityEditor : EditorWindow {

    public GameObject building;
    public List<GameObject> buildings;
    Vector2 scrollPos;

    [MenuItem("Window/Custom Utilities")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(UtilityEditor));
    }

    void OnGUI()
    {
        GUILayout.Label("Base Settings", EditorStyles.boldLabel);

        building = (GameObject)EditorGUILayout.ObjectField(building, typeof(GameObject), true);

        if(building && !buildings.Contains(building))
        {
            if (GUILayout.Button("Add Building"))
            {
                buildings.Add(building);
            }
        }

        GUILayout.Label("Buildings", EditorStyles.boldLabel);
  
        if (buildings.Count > 0)
        {

            EditorGUILayout.BeginVertical();
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(100), GUILayout.Height(100));
                foreach(GameObject go in buildings)
            {
                EditorGUILayout.ObjectField(go, typeof(GameObject), true);
            }
            EditorGUILayout.EndScrollView();


            if(GUILayout.Button("Place Buildings"))
            {
                int index = Random.Range(0, buildings.Count);
                GameObject.Instantiate(buildings[index]);
            }
        }
    }
}
