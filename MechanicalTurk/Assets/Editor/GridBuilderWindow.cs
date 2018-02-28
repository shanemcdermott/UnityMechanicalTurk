using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;




public class GridBuilderWindow : EditorWindow
{

    private Vector2 polyScrollPos;
    private PolyGrid polyGrid;

    private int tab = 0;
    private string[] typeNames;
    private GridWindow[] gridWindows;

    private bool showGridProperties;
    private bool showAdvancedOptions;

    [MenuItem("GameObject/Create Grid")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(GridBuilderWindow));
    }


    private void OnEnable()
    {
        gridWindows = new GridWindow[1];
        gridWindows[0] = new SquareGridWindow();
        typeNames = new string[gridWindows.Length];
        for(int i =0; i < gridWindows.Length; i++)
        {
            typeNames[i] = gridWindows[i].GetName();
        }
        showGridProperties = false;
    }

    private void OnGUI()
    {
        tab = GUILayout.Toolbar(tab, typeNames);

        gridWindows[tab].ShowParams();

        if (polyGrid == null)
        {
            if (GUILayout.Button("Create"))
            {
                gridWindows[tab].CreateGrid(out polyGrid);
            }
        }
        else
        {
            showGridProperties = EditorGUILayout.Foldout(showGridProperties, "Grid Properties");
            if (showGridProperties)
            {
                ShowGridProps();
            }


            showAdvancedOptions = EditorGUILayout.Foldout(showAdvancedOptions, "Advanced");
            if(showAdvancedOptions)
            {
                ShowAdvancedOptions();
            }

        }
    }

    public void ShowGridProps()
    {
            EditorGUILayout.BeginVertical();
            polyScrollPos = EditorGUILayout.BeginScrollView(polyScrollPos);
            List<Node> verts = polyGrid.GetVertices();
            GUI.enabled = false;
            for (int i = 0; i < verts.Count; i++)
            {
                string str = "Vertex " + i;
                EditorGUILayout.Vector2Field(str, verts[i].GetPosition());

                EditorGUILayout.IntField("connections", verts[i].NumConnections());

            }
            GUI.enabled = true;
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
    }

    public void ShowAdvancedOptions()
    {
        if (GUILayout.Button("Flip Y and Z"))
        {
            polyGrid.FlipAxes();
        }

        if (GUILayout.Button("Destroy"))
        {
            DestroyImmediate(polyGrid.gameObject);
        }
    }

    public void OnInspectorUpdate()
    {
        this.Repaint();
    }
}
