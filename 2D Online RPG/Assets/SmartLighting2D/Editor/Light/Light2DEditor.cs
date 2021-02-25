using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.Linq;
using LightingSettings;

[CanEditMultipleObjects]
[CustomEditor(typeof(Light2D))]
public class Light2DEditor : Editor {
	Light2D light2D;

	SerializedProperty lightPresetId;
	SerializedProperty eventPresetId;
	SerializedProperty nightLayer;

	SerializedProperty color;

	SerializedProperty size;
	SerializedProperty spotAngle;
	SerializedProperty outerAngle;

	SerializedProperty litMode;
	SerializedProperty maskTranslucency;
	SerializedProperty maskTranslucencyIntensity;

	SerializedProperty textureSize;

	SerializedProperty lightSprite;
	SerializedProperty spriteFlipX;
	SerializedProperty spriteFlipY;
	SerializedProperty sprite;

	private bool foldoutSprite = false;
	private bool foldoutBumpMap = false;

	private void InitProperties() {
		lightPresetId = serializedObject.FindProperty("lightPresetId");
		eventPresetId = serializedObject.FindProperty("eventPresetId");
		nightLayer = serializedObject.FindProperty("nightLayer");

		color = serializedObject.FindProperty("color");

		size = serializedObject.FindProperty("size");
		spotAngle = serializedObject.FindProperty("spotAngle");
		outerAngle = serializedObject.FindProperty("outerAngle");

		litMode = serializedObject.FindProperty("litMode");

		textureSize = serializedObject.FindProperty("textureSize");

		lightSprite = serializedObject.FindProperty("lightSprite");
		spriteFlipX = serializedObject.FindProperty("spriteFlipX");
		spriteFlipY = serializedObject.FindProperty("spriteFlipY");
		sprite = serializedObject.FindProperty("sprite");

		maskTranslucency = serializedObject.FindProperty("maskTranslucency");

		maskTranslucencyIntensity = serializedObject.FindProperty("maskTranslucencyIntensity");
	}

	private void OnEnable(){
		light2D = target as Light2D;

		InitProperties();
		
		Undo.undoRedoPerformed += RefreshAll;
	}

	internal void OnDisable(){
		Undo.undoRedoPerformed -= RefreshAll;
	}

	void RefreshAll(){
		Light2D.ForceUpdateAll();
	}

	override public void OnInspectorGUI() {
		if (light2D == null) {
			return;
		}

		lightPresetId.intValue = EditorGUILayout.Popup("Light Preset", lightPresetId.intValue, Lighting2D.Profile.lightPresets.GetBufferLayers());
		
		eventPresetId.intValue = EditorGUILayout.Popup("Event Preset", eventPresetId.intValue, Lighting2D.Profile.eventPresets.GetBufferLayers());

		nightLayer.intValue = EditorGUILayout.Popup("Layer (Night)", nightLayer.intValue, Lighting2D.Profile.layers.nightLayers.GetNames());
		
		EditorGUILayout.Space();

		Color colorValue = color.colorValue;

		#if UNITY_2018_1_OR_NEWER
			if (Lighting2D.QualitySettings.HDR) {
				colorValue = EditorGUILayout.ColorField(new GUIContent("Color"), colorValue, true, true, true);
			} else {
				colorValue = EditorGUILayout.ColorField("Color", colorValue);
			}
		#else
			colorValue = EditorGUILayout.ColorField("Color", colorValue);
		#endif

		colorValue.a = EditorGUILayout.Slider("Alpha", colorValue.a, 0, 1);

		color.colorValue = colorValue;

		EditorGUILayout.Space();


		size.floatValue = EditorGUILayout.Slider("Size", size.floatValue, 0.1f, Lighting2D.ProjectSettings.MaxLightSize);

		light2D.coreSize = EditorGUILayout.Slider("Core Size", light2D.coreSize, 0.1f, 10f);
	
		spotAngle.floatValue = EditorGUILayout.Slider("Spot Angle", spotAngle.floatValue, 0, 360);

		// Only Legacy Shadow
		outerAngle.floatValue = EditorGUILayout.Slider("Outer Angle", outerAngle.floatValue, 0, 60);

		EditorGUILayout.Space();

		maskTranslucency.floatValue = EditorGUILayout.Slider("Mask Translucency", maskTranslucency.floatValue, 0, 10);

		maskTranslucencyIntensity.floatValue = EditorGUILayout.Slider("Mask Translucency Intensity", maskTranslucencyIntensity.floatValue, 0.05f, 2f);

		EditorGUILayout.Space();

		EditorGUILayout.PropertyField(litMode, new GUIContent ("Lit Mode"));

		EditorGUILayout.Space();

		EditorGUI.BeginDisabledGroup(Lighting2D.Profile.qualitySettings.lightTextureSize != LightingSettings.LightingSourceTextureSize.Custom);
		
		// EditorGUILayout.PropertyField(textureSize, new GUIContent ("Buffer Size"));

		textureSize.intValue = EditorGUILayout.Popup("Buffer Size", (int)Lighting2D.Profile.qualitySettings.lightTextureSize, LightingSettings.QualitySettings.LightingSourceTextureSizeArray);
		
		EditorGUI.EndDisabledGroup();
		

		EditorGUILayout.Space();


		EditorGUILayout.Space();

		foldoutSprite = EditorGUILayout.Foldout(foldoutSprite, "Sprite" );

		if (foldoutSprite) {
			EditorGUI.indentLevel++;

			EditorGUILayout.PropertyField(lightSprite, new GUIContent ("Type"));

			//script.lightSprite = (Light2D.LightSprite)EditorGUILayout.EnumPopup("Light Sprite", script.lightSprite);
	
			if (light2D.lightSprite == Light2D.LightSprite.Custom) {
				EditorGUILayout.PropertyField(spriteFlipX, new GUIContent ("Flip X"));
				EditorGUILayout.PropertyField(spriteFlipY, new GUIContent ("Flip Y"));
				
				sprite.objectReferenceValue = (Sprite)EditorGUILayout.ObjectField("", sprite.objectReferenceValue, typeof(Sprite), true);

			} else {
				if (light2D.sprite != Light2D.GetDefaultSprite()) {
					light2D.sprite = Light2D.GetDefaultSprite();
				}
			}
		
			EditorGUI.indentLevel--;
		}

		EditorGUILayout.Space();

		GUIMeshMode.Draw(serializedObject, light2D.meshMode);

		// NOT Serialized properly yet!

		EditorGUILayout.Space();

		foldoutBumpMap = EditorGUILayout.Foldout(foldoutBumpMap, "Bump Map" );
		if (foldoutBumpMap) {
			EditorGUI.indentLevel++;

			light2D.bumpMap.intensity = EditorGUILayout.Slider("Intensity", light2D.bumpMap.intensity, 0, 2);
			light2D.bumpMap.depth = EditorGUILayout.Slider("Depth", light2D.bumpMap.depth, 0.1f, 20f);

			EditorGUI.indentLevel--;
		}

		EditorGUILayout.Space();
		
		light2D.applyRotation = EditorGUILayout.Toggle("Apply Rotation", light2D.applyRotation);

		EditorGUILayout.Space();
	
		light2D.whenInsideCollider = (Light2D.WhenInsideCollider)EditorGUILayout.EnumPopup("When Inside Collider", light2D.whenInsideCollider);

	
		serializedObject.ApplyModifiedProperties();	

	
		if (GUI.changed){
			foreach(Object target in targets) {
				Light2D light2D = target as Light2D;
				light2D.ForceUpdate();

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
