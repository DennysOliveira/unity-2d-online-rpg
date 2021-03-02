using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using LightingSettings;

[CanEditMultipleObjects]
[CustomEditor(typeof(DayLightCollider2D))]
public class DayLightCollider2DEditor : Editor {
	DayLightCollider2D dayLightCollider2D;

	SerializedProperty shadowType;
	SerializedProperty shadowLayer;
	SerializedProperty shadowDistance;
	SerializedProperty shadowTranslucency;

	SerializedProperty maskType;
	SerializedProperty maskLayer;

	SerializedProperty applyToChildren;
	
	private void InitProperties() {
		shadowType = serializedObject.FindProperty("shadowType");
		shadowLayer = serializedObject.FindProperty("shadowLayer");
		shadowDistance = serializedObject.FindProperty("shadowDistance");
		shadowTranslucency = serializedObject.FindProperty("shadowTranslucency");

		maskType = serializedObject.FindProperty("maskType");
		maskLayer = serializedObject.FindProperty("maskLayer");

		applyToChildren = serializedObject.FindProperty("applyToChildren");
	}

	private void OnEnable(){
		dayLightCollider2D = target as DayLightCollider2D;

		InitProperties();
		
		Undo.undoRedoPerformed += RefreshAll;
	}

	internal void OnDisable(){
		Undo.undoRedoPerformed -= RefreshAll;
	}

	void RefreshAll(){
		DayLightCollider2D.ForceUpdateAll();
	}

	static public bool foldoutbumpedSprite = false;

	override public void OnInspectorGUI() {
		DayLightCollider2D script = target as DayLightCollider2D;

		// Shadow Properties
		EditorGUILayout.PropertyField(shadowType, new GUIContent ("Shadow Type"));

		EditorGUI.BeginDisabledGroup(script.mainShape.shadowType == DayLightCollider2D.ShadowType.None);
		
			shadowLayer.intValue = EditorGUILayout.Popup("Shadow Layer (Day)", shadowLayer.intValue, Lighting2D.Profile.layers.dayLayers.GetNames());
			
			EditorGUILayout.PropertyField(shadowDistance, new GUIContent ("Shadow Distance"));

			EditorGUILayout.PropertyField(shadowTranslucency, new GUIContent ("Shadow Translucency"));

		EditorGUI.EndDisabledGroup();

		EditorGUILayout.Space();

		// Mask Properties

		EditorGUILayout.PropertyField(maskType, new GUIContent ("Mask Type"));
		
		EditorGUI.BeginDisabledGroup(script.mainShape.maskType == DayLightCollider2D.MaskType.None);

			maskLayer.intValue = EditorGUILayout.Popup("Mask Layer (Day)", maskLayer.intValue, Lighting2D.Profile.layers.dayLayers.GetNames());
			
			if (script.mainShape.maskType == DayLightCollider2D.MaskType.BumpedSprite) {
				GUIBumpMapMode.DrawDay(script.normalMapMode);
			}

		EditorGUI.EndDisabledGroup();

		EditorGUILayout.Space();

		// Other Properties

		EditorGUILayout.PropertyField(applyToChildren, new GUIContent ("Apply To Children"));

		EditorGUILayout.Space();

		serializedObject.ApplyModifiedProperties();
		
		if (GUILayout.Button("Update")) {
			SpriteExtension.PhysicsShapeManager.Clear();

			foreach(Object target in targets) {
				DayLightCollider2D daylightCollider2D = target as DayLightCollider2D;
				
				daylightCollider2D.mainShape.ResetLocal();

				daylightCollider2D.Initialize();
			}
		}

		if (GUI.changed) {
			foreach(Object target in targets) {
				DayLightCollider2D daylightCollider2D = target as DayLightCollider2D;
				daylightCollider2D.Initialize();

				if (EditorApplication.isPlaying == false) {
					EditorUtility.SetDirty(target);
				}
			}

			if (EditorApplication.isPlaying == false) {
           		EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
			}
		}
	}
}
