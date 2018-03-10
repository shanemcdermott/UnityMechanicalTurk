using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor (typeof (MapGenerator))]
public class MapGeneratorEditor : Editor {

	public override void OnInspectorGUI() {
		MapGenerator mapGen = (MapGenerator)target;
		if (DrawDefaultInspector ()) {
			if (mapGen.autoUpdate) {
                mapGen.Setup();
				mapGen.Generate();
			}
		}

		if (GUILayout.Button ("Generate")) {
            mapGen.Setup();
            if (mapGen.CanGenerate())
			    mapGen.Generate();
		}
	}
}
