using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.Linq;


[CanEditMultipleObjects]
[CustomEditor(typeof(LightMesh2D))]
public class LightMesh2DEditor : Editor {

    override public void OnInspectorGUI() {
        LightMesh2D script = target as LightMesh2D;

        script.nightLayer = EditorGUILayout.Popup("Layer (Night)", script.nightLayer, Lighting2D.Profile.layers.nightLayers.GetNames());

        script.lightLayer = EditorGUILayout.Popup("Layer (Light)", script.lightLayer, Lighting2D.Profile.layers.lightLayers.GetNames());

        EditorGUILayout.Space();

        script.color = EditorGUILayout.ColorField("Color", script.color);

        script.color.a = EditorGUILayout.Slider("Alpha", script.color.a, 0, 1);

        EditorGUILayout.Space();

        script.size = EditorGUILayout.FloatField("Size", script.size);

        if (script.size < 0.1f) {
            script.size = 0.1f;
        }

        script.segments = EditorGUILayout.IntSlider("Vertices Count", script.segments, 6, 60);

        EditorGUILayout.Space();

        bool value = GUIFoldout.Draw("Sprite", script);
        
		if (value) {
            EditorGUI.indentLevel++;

            script.useUVColor = EditorGUILayout.Toggle("Use UV Color", script.useUVColor);

            script.useUV = EditorGUILayout.Toggle("Use UV", script.useUV);

            EditorGUI.BeginDisabledGroup(script.useUV == false);

            script.sprite = (Sprite)EditorGUILayout.ObjectField("Sprite", script.sprite, typeof(Sprite), true);
            
            EditorGUI.EndDisabledGroup();

            EditorGUI.indentLevel--;
		}

        EditorGUILayout.Space();
        
        GUIMeshMode.Draw(serializedObject, script.meshMode);

        serializedObject.ApplyModifiedProperties();

        if (GUI.changed){
			script.UpdateLoop();

             if (EditorApplication.isPlaying == false) {
				EditorUtility.SetDirty(target);
            	EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
			}
		}
    }
   
}
