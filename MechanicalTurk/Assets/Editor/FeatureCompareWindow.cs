using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Framework.Util;

public class FeatureCompareWindow : EditorWindow
{
    protected GameObject camControllerObj;
    protected CameraController camController;
    protected float viewRatio = 0.5f;

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

        ShowCamControllerPanel();

    }

    protected void ShowCamControllerPanel()
    {
        GUILayout.Label("Camera Controller", boldStyle);
        EditorGUILayout.BeginHorizontal();
            camControllerObj = (GameObject)EditorGUILayout.ObjectField(camControllerObj, typeof(GameObject), true);

            if (camControllerObj == null)
            {
                if (GUILayout.Button("Create"))
                {
                    camControllerObj = new GameObject("CameraController");
                    camController = camControllerObj.AddComponent<CameraController>();

                    GameObject camAobj = new GameObject("Camera A");
                    camAobj.transform.SetParent(camControllerObj.transform);

                    GameObject camBobj = new GameObject("Camera B");
                    camBobj.transform.SetParent(camControllerObj.transform);

                    camController.cameraA = camAobj.AddComponent<Camera>();
                    camController.cameraA.depth = -1;
             
                    camController.cameraB = camBobj.AddComponent<Camera>();
                    camController.cameraB.depth = 0;

                    SetViewRatios();
                }
            }
            else
            {

                if (GUILayout.Button("Destroy"))
                {
                    GameObject.DestroyImmediate(camControllerObj);
                }
            }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Camera A", italicStyle);
        GUILayout.Label("Camera B", italicStyle);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginVertical();
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            ShowViewRatioControls();
            ShowViewTargetPanel();


        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
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

    private void ShowViewTargetPanel()
    {
        GUILayout.Label("View Targets");
       
    }


    private void SetViewRatios()
    {
        if (camController == null) return;

        if(camController.cameraA)
            camController.cameraA.rect = new Rect(new Vector2(0, 0), new Vector2(viewRatio, 1f));

        if(camController.cameraB)
            camController.cameraB.rect = new Rect(new Vector2(viewRatio, 0), new Vector2(1 - viewRatio, 1f));
    }
}
