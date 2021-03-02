using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

[CanEditMultipleObjects]
[CustomEditor(typeof(LightCollider2D))]
public class LightCollider2DEditor : Editor {
	LightCollider2D lightCollider2D;

	SerializedProperty shadowType;
	SerializedProperty shadowLayer;
	SerializedProperty shadowDistance;
	SerializedProperty shadowTranslucency;
	
	SerializedProperty maskType;
	SerializedProperty maskLayer;
	SerializedProperty maskEffect;
	SerializedProperty maskPivot;
	SerializedProperty maskTranslucency;

	SerializedProperty applyToChildren;

	private void InitProperties() {
		shadowType = serializedObject.FindProperty("shadowType");
		shadowLayer = serializedObject.FindProperty("shadowLayer");
		shadowDistance = serializedObject.FindProperty("shadowDistance");
		shadowTranslucency = serializedObject.FindProperty("shadowTranslucency");
		
		maskType = serializedObject.FindProperty("maskType");
		maskLayer = serializedObject.FindProperty("maskLayer");
		maskEffect = serializedObject.FindProperty("maskEffect");
		maskPivot = serializedObject.FindProperty("maskPivot");
		maskTranslucency = serializedObject.FindProperty("maskTranslucency");

		applyToChildren = serializedObject.FindProperty("applyToChildren");
	}

	private void OnEnable(){
		lightCollider2D = target as LightCollider2D;

		InitProperties();
		
		Undo.undoRedoPerformed += RefreshAll;
	}

	internal void OnDisable(){
		Undo.undoRedoPerformed -= RefreshAll;
	}

	void RefreshAll(){
		LightCollider2D.ForceUpdateAll();
	}

	override public void OnInspectorGUI() {
		if (lightCollider2D == null) {
			return;
		}

		// Warning
		// Debug.Log(lightCollider2D.mainShape.spriteShape.GetOriginalSprite().packingMode);
		
		// Shadow Properties

		EditorGUILayout.PropertyField(shadowType, new GUIContent ("Shadow Type"));

		EditorGUI.BeginDisabledGroup(shadowType.intValue == (int)LightCollider2D.ShadowType.None);

		shadowLayer.intValue = EditorGUILayout.Popup("Shadow Layer (Light)", shadowLayer.intValue, Lighting2D.Profile.layers.lightLayers.GetNames());

		string shadowDistanceName = "Shadow Distance";

		if (shadowDistance.floatValue == 0) {
			shadowDistanceName = "Shadow Distance (infinite)";
		}

		EditorGUILayout.PropertyField(shadowDistance, new GUIContent (shadowDistanceName));

		EditorGUILayout.PropertyField(shadowTranslucency, new GUIContent ("Shadow Translucency"));

		EditorGUI.EndDisabledGroup();

		EditorGUILayout.Space();

		// Mask Properties

		EditorGUILayout.PropertyField(maskType, new GUIContent ("Mask Type"));

		EditorGUI.BeginDisabledGroup(maskType.intValue == (int)LightCollider2D.MaskType.None);

		maskLayer.intValue = EditorGUILayout.Popup("Mask Layer (Light)", maskLayer.intValue, Lighting2D.Profile.layers.lightLayers.GetNames());

		EditorGUILayout.PropertyField(maskEffect, new GUIContent ("Mask Effect"));

		EditorGUILayout.PropertyField(maskPivot, new GUIContent ("Mask Pivot"));

		EditorGUILayout.PropertyField(maskTranslucency, new GUIContent ("Mask Translucency"));

		EditorGUI.EndDisabledGroup();

		if (lightCollider2D.maskType == LightCollider2D.MaskType.BumpedSprite || lightCollider2D.maskType == LightCollider2D.MaskType.BumpedMeshRenderer) {
			GUIBumpMapMode.Draw(serializedObject, lightCollider2D);
		}

		EditorGUILayout.Space();

		// Apply to Children
		
		EditorGUILayout.PropertyField(applyToChildren, new GUIContent ("Apply to Children"));

		serializedObject.ApplyModifiedProperties();


		EditorGUILayout.Space();

		// Update

		if (GUILayout.Button("Update")) {
			SpriteExtension.PhysicsShapeManager.Clear();

			foreach(Object target in targets) {
				LightCollider2D lightCollider2D = target as LightCollider2D;
				lightCollider2D.Initialize();
			}

			LightingManager2D.ForceUpdate();
		}

		if (GUI.changed) {
			foreach(Object target in targets) {
				LightCollider2D lightCollider2D = target as LightCollider2D;
				lightCollider2D.Initialize();
				lightCollider2D.UpdateNearbyLights();

				if (EditorApplication.isPlaying == false) {
					EditorUtility.SetDirty(target);
				}
			}

			if (EditorApplication.isPlaying == false) {
				EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
			}

			LightingManager2D.ForceUpdate();
		}
	}
}
