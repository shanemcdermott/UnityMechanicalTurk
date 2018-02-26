using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof (PolyGrid))]
public class PolyGraphEditor : Editor
{
	Node node;

	public override void OnInspectorGUI()
	{
		DrawDefaultInspector ();
		PolyGrid grid = (PolyGrid)target;
		node = (Node)EditorGUILayout.ObjectField (node, typeof(Node),true);
		if (GUILayout.Button ("AddVertex")) {
			Node nunode = GameObject.Instantiate (node, grid.transform);
			grid.AddVertex (nunode);
		}
	}
}
