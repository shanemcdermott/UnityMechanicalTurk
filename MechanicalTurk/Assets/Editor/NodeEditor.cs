using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Node))]
public class NodeEditor : Editor
{
    public Node obj = null;
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Node node = (Node)target;
        EditorGUILayout.IntField("Connections", node.NumConnections());

        obj = (Node)EditorGUILayout.ObjectField(obj, typeof(Node),true);
        if (obj)
        {
            if (node.IsConnectedTo(obj))
            {
                DrawConnectedOptions(node);
            }
            else if (GUILayout.Button("Connect Nodes"))
            {
                node.AddConnection(obj);
            }

        }
    }

    public void DrawConnectedOptions(Node node)
    {
        if (GUILayout.Button("Disconnect Nodes"))
        {
            node.RemoveConnection(obj);
        }
    }

}
