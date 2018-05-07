using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEditor;

using CameraControls;

/// <summary>
/// Editor Panel for CameraViewTarget
/// </summary>
[CustomEditor(typeof(CameraViewTarget))]
public class CameraViewTargetEditor : Editor
{

    private CameraViewTarget _viewTarget;

    private SerializedProperty _viewPosition;
    private SerializedProperty _viewRotation;

    private SerializedProperty _duration;
    private SerializedProperty _nextView;

    private void OnEnable()
    {
        _viewTarget = (CameraViewTarget)serializedObject.targetObject;
        _nextView = serializedObject.FindProperty("nextView");
        _viewPosition = serializedObject.FindProperty("viewPosition");
        _viewRotation = serializedObject.FindProperty("viewRotation");
        _duration = serializedObject.FindProperty("duration");
    }

    public override void OnInspectorGUI()
    {
        
        if (GUILayout.Button("Align with Scene View"))
        {
            AlignWithSceneView();
        }


        serializedObject.Update();
        EditorGUILayout.PropertyField(_viewPosition);
        EditorGUILayout.PropertyField(_viewRotation);
        EditorGUILayout.PropertyField(_duration);
        EditorGUILayout.PropertyField(_nextView);
        if(_viewTarget.nextView == null && GUILayout.Button("Create Next From Scene View"))
        {
            CreateNextViewTarget();
        }
        serializedObject.ApplyModifiedProperties();

        if (GUILayout.Button("View in Scene"))
        {
            AlignSceneViewWithTarget();
        }
    }

    public void ShowPropertiesAt(int sequencePosition)
    {
        if (sequencePosition > 0)
        {
            CameraViewTarget previewTarget = _viewTarget.nextView;
            for (int i = 1; i < sequencePosition; i++)
            {
                if (previewTarget == null) return;

                previewTarget = previewTarget.nextView;
            }

            CreateEditor(previewTarget).OnInspectorGUI();
        }
    }

    /// <summary>
    /// Sets editor scene view to match the location and rotation of the currently selected CameraViewTarget
    /// </summary>
    public void AlignSceneViewWithTarget()
    {
        if(SceneView.lastActiveSceneView != null)
        {
            SceneView.lastActiveSceneView.pivot = _viewTarget.viewPosition;
            SceneView.lastActiveSceneView.rotation =  Quaternion.Euler(_viewTarget.viewRotation);
        }
    }

    /// <summary>
    /// Aligns the currently selected CameraViewTarget's position and rotation with the current SceneView
    /// </summary>
    public void AlignWithSceneView()
    {
        _viewTarget.viewPosition = SceneView.lastActiveSceneView.pivot;
        _viewTarget.viewRotation = SceneView.lastActiveSceneView.rotation.eulerAngles;
    }

    /// <summary>
    /// Creates a new CameraViewTarget from the editor's current scene view and
    /// sets it as the currently selected CameraViewTarget's next view.
    /// </summary>
    public void CreateNextViewTarget()
    {
        CameraViewTarget nextViewtarget = ScriptableObject.CreateInstance<CameraViewTarget>();
        nextViewtarget.viewPosition = SceneView.lastActiveSceneView.pivot;
        nextViewtarget.viewRotation = SceneView.lastActiveSceneView.rotation.eulerAngles;
        _viewTarget.nextView = nextViewtarget;

        string newName = AssetDatabase.GetAssetPath(_viewTarget);
        int i = newName.LastIndexOf('_');
        int k = newName.LastIndexOf('.');
        newName = newName.Substring(0, k);
        if(i>0)
        {
            int j;
            if(Int32.TryParse(newName.Substring(i + 1),out j))
            {
                j++;
                newName = newName.Substring(0, i + 1) + j.ToString();
            }
            else
            {
                newName += "0";
            }
        }
        else
        {
            newName += "0";
        }

        string createPath = newName + ".asset";
        AssetDatabase.CreateAsset(nextViewtarget, createPath);

        Debug.Log("View Target created at " + AssetDatabase.GetAssetPath(nextViewtarget));
    }

    [MenuItem("GameObject/Camera Controller/View Target")]
    public static CameraViewTarget CreateViewTarget()
    {
        int i = AssetDatabase.FindAssets("CameraViewTarget").Length;
        CameraViewTarget nextViewtarget = ScriptableObject.CreateInstance<CameraViewTarget>();
        nextViewtarget.viewPosition = SceneView.lastActiveSceneView.pivot;
        nextViewtarget.viewRotation = SceneView.lastActiveSceneView.rotation.eulerAngles;

        AssetDatabase.CreateAsset(nextViewtarget, "Assets/CameraViewTarget_" + i + ".asset");

        Debug.Log("View Target created at " + AssetDatabase.GetAssetPath(nextViewtarget));

        return nextViewtarget;
    }
}
