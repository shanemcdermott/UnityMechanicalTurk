using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Framework.Util;
using Algorithms.Util;

public class FeatureCompareWindow : EditorWindow
{


    public int typeTab;
    public string[] typeTabs = new string[] { "Standard", "Algorithm"};

    protected GameObject camControllerObj;
    protected CameraController camController;
    protected float viewRatio = 0.5f;


    protected bool showViewPanelControls = false;
    protected int viewCameraId;
    protected float viewDuration = 5;
    protected Vector3 viewLocation;
    protected Vector3 viewRotation;



    protected Vector2 scrollPos;
    GUIStyle boldStyle;
    GUIStyle italicStyle;

    [MenuItem("Window/Feature Comparison")]
    public static void ShowWindow()
    {
       
        EditorWindow.GetWindow(typeof(FeatureCompareWindow));
    }


    private void OnGUI()
    {
        boldStyle = new GUIStyle();
        boldStyle.fontStyle = FontStyle.Bold;
        italicStyle = new GUIStyle();
        italicStyle.fontStyle = FontStyle.Italic;
        italicStyle.wordWrap = true;

        typeTab = GUILayout.Toolbar(typeTab, typeTabs);
        switch (typeTab)
        {
            case 1:
                ShowAlgoCamControllerPanel();
                break;

            default:
                ShowCamControllerPanel();
                break;

        }

    }

    protected void ShowAlgoCamControllerPanel()
    {
        EditorGUILayout.LabelField("Advanced Camera Controller that iterates through GenerationController results.", italicStyle);
        EditorGUILayout.BeginHorizontal();
        camController = (GeneratorCameraController)EditorGUILayout.ObjectField(camController, typeof(GeneratorCameraController), true);

        if (camController == null)
        {
            if (GUILayout.Button("Create"))
            {
                camControllerObj = new GameObject("CameraController");
                camController = camControllerObj.AddComponent<GeneratorCameraController>();
                camController.AddCameras(2);
                SetViewRatios();
            }
        }
        EditorGUILayout.EndHorizontal();


        if (camController != null)
            ShowCameraOptions();
    }

    protected void ShowCamControllerPanel()
    {
        EditorGUILayout.LabelField("Standard Camera Controller that cycles indefinitely.", italicStyle);
        EditorGUILayout.BeginHorizontal();
            camController = (CameraController)EditorGUILayout.ObjectField(camController, typeof(CameraController), true);

            if (camController == null)
            {
                if (GUILayout.Button("Create"))
                {
                    camControllerObj = new GameObject("CameraController");
                    camController = camControllerObj.AddComponent<CameraController>();
                    camController.AddCameras(2);
                    SetViewRatios();
                }
            }
        EditorGUILayout.EndHorizontal();

        if(camController != null)
            ShowCameraOptions();

    }

    private void ShowCameraOptions()
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Camera A", italicStyle);
        GUILayout.Label("Camera B", italicStyle);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginVertical();
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        ShowViewRatioControls();
        showViewPanelControls = EditorGUILayout.Foldout(showViewPanelControls, "Add Camera View");
        if (showViewPanelControls)
        {
            ShowViewPanel();
        }


        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }


    private void ShowViewPanel()
    {

        viewCameraId = EditorGUILayout.IntField("Camera ID", viewCameraId);
        viewDuration = EditorGUILayout.FloatField("View Duration", viewDuration);

        Transform camViewTransform = (Transform)EditorGUILayout.ObjectField("Target transform", null, typeof(Transform), true);
        viewLocation = EditorGUILayout.Vector3Field("View Position", viewLocation);
        viewRotation = EditorGUILayout.Vector3Field("View Rotation", viewRotation);
        if(camViewTransform)
        {
            viewLocation = camViewTransform.position;
            viewRotation = camViewTransform.rotation.eulerAngles;
        }
        if(GUILayout.Button("Create View"))
        {
            camController.AddCameraView(viewCameraId, viewDuration, viewLocation, viewRotation);
        }
    }

    private void ShowViewRatioControls()
    {
        GUILayout.Label("View Ratio");

        float newRatio = EditorGUILayout.Slider(viewRatio, 0.0f, 1.0f);


        if (newRatio != viewRatio)
        {
            viewRatio = newRatio;
            SetViewRatios();
        }
    }


    private void SetViewRatios()
    {
        if (camController == null) return;

        if(camController.cameras.Length > 0)
            camController.cameras[0].rect = new Rect(new Vector2(0, 0), new Vector2(viewRatio, 1f));

        if(camController.cameras.Length > 1)
            camController.cameras[1].rect = new Rect(new Vector2(viewRatio, 0), new Vector2(1 - viewRatio, 1f));
    }
}
