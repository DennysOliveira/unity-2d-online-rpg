using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

[CanEditMultipleObjects]
[CustomEditor(typeof(LightTexture2D))]
public class LightTexture2DEditor : Editor {

	override public void OnInspectorGUI() {
		LightTexture2D script = target as LightTexture2D;

		script.nightLayer = EditorGUILayout.Popup("Layer (Night)", script.nightLayer, Lighting2D.Profile.layers.nightLayers.GetNames());

		script.shaderMode = (LightTexture2D.ShaderMode)EditorGUILayout.EnumPopup("Shader Mode", script.shaderMode);

		script.color = EditorGUILayout.ColorField("Color", script.color);

		script.size = EditorGUILayout.Vector2Field("Size", script.size);

		script.texture = (Texture)EditorGUILayout.ObjectField("Texture", script.texture, typeof(Texture), true);

		if (GUI.changed){
			
			if (EditorApplication.isPlaying == false) {
				EditorUtility.SetDirty(target);
				EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
			}
		}
	}
}
