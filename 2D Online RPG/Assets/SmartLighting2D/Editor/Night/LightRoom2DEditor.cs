using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

[CustomEditor(typeof(LightRoom2D))]
public class LightRoom2DEditor : Editor {
    override public void OnInspectorGUI() {
		LightRoom2D script = target as LightRoom2D;

		script.nightLayer = EditorGUILayout.Popup("Night Layer", script.nightLayer, Lighting2D.Profile.layers.nightLayers.GetNames());

		script.shape.type = (LightRoom2D.RoomType)EditorGUILayout.EnumPopup("Room Type", script.shape.type);

 		script.color = EditorGUILayout.ColorField("Color", script.color);

		Update(); 

		if (GUI.changed) {
			if (EditorApplication.isPlaying == false) {
				EditorUtility.SetDirty(script);
				EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());

				LightingManager2D.ForceUpdate();
			}
		}
	}

	void Update() {
		LightRoom2D script = target as LightRoom2D;

		if (GUILayout.Button("Update")) {
			script.Initialize();
		}
	}
}
