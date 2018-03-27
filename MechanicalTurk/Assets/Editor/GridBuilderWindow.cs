using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;




public class GridBuilderWindow : EditorWindow
{


    private PolyGrid polyGrid;

    private int tab = 0;
    private string[] typeNames;
    private GridWindow[] gridWindows;

    private bool showGridProperties;
    private bool showGridVertices;
    private Vector2 vertScrollPos;
    private bool showGridFaces;
    private Vector2 faceScrollPos;
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

        if(polyGrid.GetComponent<MeshFilter>())
        {
            if(GUILayout.Button("Save Mesh"))
            {
                SaveMesh(polyGrid.GetComponent<MeshFilter>().sharedMesh, "Test Mesh", true, true);
            }
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

    public static void SaveMesh(Mesh mesh, string name, bool makeNewInstance, bool optimizeMesh)
    {
        string path = EditorUtility.SaveFilePanel("Save Separate Mesh Asset", "Assets/", name, "asset");
        if (string.IsNullOrEmpty(path)) return;

        path = FileUtil.GetProjectRelativePath(path);

        Mesh meshToSave = (makeNewInstance) ? Object.Instantiate(mesh) as Mesh : mesh;

        if (optimizeMesh)
            MeshUtility.Optimize(meshToSave);

        AssetDatabase.CreateAsset(meshToSave, path);
        AssetDatabase.SaveAssets();
    }
}
