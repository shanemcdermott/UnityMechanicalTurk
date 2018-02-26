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
            EditorGUILayout.BeginVertical();
            polyScrollPos = EditorGUILayout.BeginScrollView(polyScrollPos);
            List<Node> verts = polyGrid.GetVertices();
            for (int i = 0; i < verts.Count; i++)
            {
                string str = "node " + i;
                EditorGUILayout.Vector2Field(str, verts[i].GetPosition());
                GUI.enabled = false;
                EditorGUILayout.IntField("connections", verts[i].NumConnections());
                GUI.enabled = true;
            }
            EditorGUILayout.EndScrollView();

        }
    }
}
