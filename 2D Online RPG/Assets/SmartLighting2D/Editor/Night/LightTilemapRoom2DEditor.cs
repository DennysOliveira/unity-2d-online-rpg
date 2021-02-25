using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using LightTilemapCollider;

#if UNITY_2017_4_OR_NEWER

[CustomEditor(typeof(LightTilemapRoom2D))]
public class LightTilemapRoom2DEditor : Editor {

	override public void OnInspectorGUI() {
		LightTilemapRoom2D script = target as LightTilemapRoom2D;

		script.nightLayer = EditorGUILayout.Popup("Night Layer", script.nightLayer, Lighting2D.Profile.layers.nightLayers.GetNames());

		EditorGUILayout.Space();

		script.mapType = (MapType)EditorGUILayout.EnumPopup("Map Type", script.mapType);

		EditorGUILayout.Space();

		script.maskType = (LightTilemapRoom2D.MaskType)EditorGUILayout.EnumPopup("Mask Type", script.maskType);
		
		EditorGUILayout.Space();

		script.shaderType = (LightTilemapRoom2D.ShaderType)EditorGUILayout.EnumPopup("Shader Type", script.shaderType);

		script.color = EditorGUILayout.ColorField("Shader Color", script.color);

		EditorGUILayout.Space();
	
		if (GUILayout.Button("Update")) {
			SpriteExtension.PhysicsShapeManager.Clear();
			
			script.Initialize();
			LightingManager2D.ForceUpdate();
		}

		if (GUI.changed) {
			// script.Initialize();

			LightingManager2D.ForceUpdate();
			
			if (EditorApplication.isPlaying == false) {
				EditorUtility.SetDirty(script);
				EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
			}
		}
	}
}

#endif