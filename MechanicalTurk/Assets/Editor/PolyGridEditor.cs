using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PolyGrid))]
public class PolyGridEditor : Editor
{
    private PolyGrid polyGrid;
    private int tab = 0;
    private string[] typeNames = new string[1] { "Square" };

    private GridWindow[] gridWindows = new GridWindow[1] {new SquareGridWindow() };

    private bool showGridProperties = true;
    private bool showGridVertices;
    private Vector2 vertScrollPos;
    private bool showGridFaces;
    private Vector2 faceScrollPos;
    private bool showAdvancedOptions;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        polyGrid = (PolyGrid)target;

        tab = GUILayout.Toolbar(tab, typeNames);

        gridWindows[tab].grid = polyGrid;
 
        if(polyGrid.NumFaces() ==0)
        {
            if (GUILayout.Button("Create Grid"))
            {
                GridFactory.PopulateSquareGrid(ref polyGrid);
            }
        }

        showGridProperties = EditorGUILayout.Foldout(showGridProperties, "Grid Properties");
        if (showGridProperties)
        {
            ShowGridProps();
        }


        showAdvancedOptions = EditorGUILayout.Foldout(showAdvancedOptions, "Advanced");
        if (showAdvancedOptions)
        {
                ShowAdvancedOptions();
        }
    }

    public void ShowGridProps()
    {
        EditorGUILayout.BeginHorizontal();

        showGridVertices = EditorGUILayout.Foldout(showGridVertices, "Vertices");
        if (showGridVertices)
        {
            vertScrollPos = EditorGUILayout.BeginScrollView(vertScrollPos);
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
        }

        showGridFaces = EditorGUILayout.Foldout(showGridFaces, "Faces");
        if (showGridFaces)
        {
            faceScrollPos = EditorGUILayout.BeginScrollView(faceScrollPos);
            GUI.enabled = false;
            List<GridFace> faces = polyGrid.GetFaces();
            for (int i = 0; i < faces.Count; i++)
            {
                string str = "Face " + i;
                EditorGUILayout.Vector2Field(str, faces[i].GetPosition());

                EditorGUILayout.IntField("vertices", faces[i].NumVertices());
            }
            GUI.enabled = true;
            EditorGUILayout.EndScrollView();
        }
        EditorGUILayout.EndHorizontal();



    }

    public void ShowAdvancedOptions()
    {
        if (GUILayout.Button("Flip Y and Z"))
        {
            polyGrid.FlipAxes();
        }
        if(GUILayout.Button("Subdivide"))
        {
            gridWindows[tab].Subdivide();
        }
    }

    public void OnInspectorUpdate()
    {
        this.Repaint();
    }
}
