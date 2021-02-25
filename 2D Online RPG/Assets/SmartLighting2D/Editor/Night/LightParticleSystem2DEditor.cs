using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;


[CanEditMultipleObjects]
[CustomEditor(typeof(LightParticleSystem2D))]
public class LightParticleSystem2DEditor : Editor {

	override public void OnInspectorGUI() {
		LightParticleSystem2D script = target as LightParticleSystem2D;

		script.nightLayer = EditorGUILayout.Popup("Layer (Night)", script.nightLayer, Lighting2D.Profile.layers.nightLayers.GetNames());

        script.color = EditorGUILayout.ColorField("Color", script.color);
        
        script.color.a = EditorGUILayout.Slider("Alpha", script.color.a, 0, 1);

        script.scale = EditorGUILayout.FloatField("Scale", script.scale);

        script.customParticle = (Texture)EditorGUILayout.ObjectField("Custom Particle", script.customParticle, typeof(Texture), true);

		if (GUI.changed){
            if (EditorApplication.isPlaying == false) {
                EditorUtility.SetDirty(target);
                EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            }
		}
	}
}
