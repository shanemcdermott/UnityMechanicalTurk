using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using Framework.Util;

[CustomEditor(typeof(CameraController))]
public class CameraControllerEditor : Editor
{
    protected CameraController _camController;

    private int _cameraTab;
    private string[] _tabNames;

    private SerializedProperty _camProperties;
    private bool showCamProperties = false;

    private SerializedProperty _seqProperties;
    private bool showViewTargetProperties = false;
    private int previewIndex = 0;

    [MenuItem("GameObject/Camera Controller/Standard")]
    public static void CreateCameraController()
    {
        GameObject camObj = new GameObject("CameraController");
        CameraController camCon = camObj.AddComponent<CameraController>();
        camCon.AddCameras(2);

        Selection.activeGameObject = camObj;
    }

    private void OnEnable()
    {
        _camController = (CameraController)target;
        UpdateCameraTabs();
    }

    private void UpdateCameraTabs()
    {
        _cameraTab = 0;
        _tabNames = new string[_camController.cameras.Length + 1];
        for(int i = 0; i < _camController.cameras.Length; i++)
        {
            _tabNames[i] = _camController.cameras[i].gameObject.name;
        }
        _tabNames[_tabNames.Length - 1] = "+";
        _camProperties = serializedObject.FindProperty("cameras");
        _seqProperties = serializedObject.FindProperty("viewSequences");
    }

    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        serializedObject.Update();

        EditorGUILayout.BeginVertical("box");
        _cameraTab = GUILayout.Toolbar(_cameraTab, _tabNames);
        if (_cameraTab == _tabNames.Length - 1)
        {
            _camController.AddCamera();
            UpdateCameraTabs();
        }
        else
        {
            ShowIndexProperties(_cameraTab);
        }
        EditorGUILayout.EndVertical();
        serializedObject.ApplyModifiedProperties();
    }

    private void ShowIndexProperties(int index)
    {
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.PropertyField(_camProperties.GetArrayElementAtIndex(index),new GUIContent("Camera"));
        showCamProperties = EditorGUILayout.Foldout(showCamProperties, "Camera Properties");
        if (showCamProperties)
        {
            Object camObj = _camProperties.GetArrayElementAtIndex(index).objectReferenceValue;
            if (camObj)
            {
                CreateEditor(camObj).OnInspectorGUI();
                if (GUILayout.Button("Remove Camera"))
                {
                    _camController.RemoveCamera(_cameraTab);
                    UpdateCameraTabs();
                }
            }
        }


        EditorGUILayout.EndVertical();
        EditorGUILayout.Separator();
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.PropertyField(_seqProperties.GetArrayElementAtIndex(index), new GUIContent("View Target"));
        CameraViewTarget viewObj = (CameraViewTarget)_seqProperties.GetArrayElementAtIndex(index).objectReferenceValue;
        if (viewObj)
        {
            showViewTargetProperties = EditorGUILayout.Foldout(showViewTargetProperties, "View Target Properties");
            if (showViewTargetProperties)
            {
                int len = viewObj.GetNumRemaining();
                EditorGUILayout.LabelField("View Sequence Length", (1 + len).ToString());
                CameraViewTargetEditor editor = (CameraViewTargetEditor)CreateEditor(viewObj);
                previewIndex = EditorGUILayout.IntSlider("Sequence Preview Index", previewIndex,0, len);
                if (previewIndex > 0)
                {
                    editor.ShowPropertiesAt(previewIndex);
                }
                else
                {
                    editor.OnInspectorGUI();
                }

            }
        }
        else if (GUILayout.Button("Create Target From Scene View"))
        {
                _seqProperties.GetArrayElementAtIndex(index).objectReferenceValue = CameraViewTargetEditor.CreateViewTarget();
        }
        EditorGUILayout.EndVertical();
    }

}
