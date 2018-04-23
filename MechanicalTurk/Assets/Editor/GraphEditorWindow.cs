using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class GraphEditorWindow : EditorWindow {

    public int tab;
    public string[] tabs = new string[] {"Grid", "Graph"};


    private GameObject dummyObject;
    private int seed = 0;

    private PolygonCollider2D collider;
    private PointGenerator pointGenerator;
    private DelaunayTriangulation delTri;


    public PolyGrid polyGrid;
    Vector2 polyScrollPos;

    [MenuItem("Window/Graph Editor")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(GraphEditorWindow));
    }

    private void OnEnable()
    {
        if (dummyObject == null)
        {
            dummyObject = GameObject.FindGameObjectWithTag("Utilities");
            if (dummyObject == null)
            {
                dummyObject = new GameObject("Utilities");
                dummyObject.tag = "Utilities";
                collider = dummyObject.AddComponent<PolygonCollider2D>();
                ResizeColliderPoints();
                pointGenerator = dummyObject.AddComponent<PoissonDiskSampling>();
                //pointGenerator.bounds = collider;
                pointGenerator.Init();
                delTri = dummyObject.AddComponent<DelaunayTriangulation>();
                delTri.bounds = collider;
                delTri.Init();
            }
        }

        if (pointGenerator == null)
        {
            pointGenerator = dummyObject.GetComponent<PoissonDiskSampling>();  
        }
        if(delTri == null)
        {
            delTri = dummyObject.GetComponent<DelaunayTriangulation>();
        }
    }

    //Draw the Tabs and the current tab
    private void OnGUI()
    {
        seed = EditorGUILayout.IntField("Seed", seed);
        if (GUILayout.Button("Set Seed"))
        {
            Random.InitState(seed);
        }

        if (GUILayout.Button("Reset"))
        {
            pointGenerator.Init();
            delTri.Init();
        }

        ShowTabs();

    }

    private void ShowTabs()
    {
        tab = GUILayout.Toolbar(tab, tabs);
        switch (tab)
        {
            case 0:
                ShowGridControls();
                break;
            case 1:
                ShowGraphControls();
                break;
        }
    }

    private void ShowGridControls()
    {
        GUILayout.Label("Grid Settings", EditorStyles.boldLabel);
        polyGrid = (PolyGrid)EditorGUILayout.ObjectField(polyGrid, typeof(PolyGrid), true);
        if(polyGrid == null)
        {
            if (GUILayout.Button("New Grid"))
            {
                polyGrid = dummyObject.AddComponent<PolyGrid>();
            }
        }
        if (polyGrid)
        {
            GUILayout.Label("Vertices");
            if (GUILayout.Button("Next Point"))
            {
                Vector2 v2 = pointGenerator.NextPoint();
                if (!v2.Equals(Vector2.positiveInfinity))
                {
                    delTri.AddPoint(v2);
                }
                else
                {
                    Debug.Log("No points found!");
                }
            }
            ListGridVertices();

        }
        
    }

    private void ListGridVertices()
    {
        EditorGUILayout.BeginVertical();
        polyScrollPos = EditorGUILayout.BeginScrollView(polyScrollPos, GUILayout.Width(100), GUILayout.Height(100));
        foreach (Node node in polyGrid.GetVertices())
        {
            EditorGUILayout.Vector3Field("position",node.GetPosition());
            
        }
        EditorGUILayout.EndScrollView();
    }

    private void ShowGraphControls()
    {

    }

    private void ResizeColliderPoints()
    {
        Vector2[] points = collider.points;
        for(int i = 0; i < points.Length; i++)
        {
            points[i] = points[i] * 100;
        }
        collider.points = points;
    }

}
