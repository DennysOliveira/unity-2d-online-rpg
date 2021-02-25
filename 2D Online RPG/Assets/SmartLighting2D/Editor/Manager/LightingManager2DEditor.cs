using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

[CustomEditor(typeof(LightingManager2D))]
public class LightingManager2DEditor : Editor {
	static bool[] cameraFoldout = new bool[10];

	static bool[] fogOfWarCameraFoldout = new bool[10];

	static bool fogOfWarFoldout = false;

	override public void OnInspectorGUI() {
		LightingManager2D script = target as LightingManager2D;

		LightingSettings.Profile newProfile = (LightingSettings.Profile)EditorGUILayout.ObjectField("Profile", script.setProfile, typeof(LightingSettings.Profile), true);
		if (newProfile != script.setProfile) {
			script.setProfile = newProfile;

			script.UpdateProfile();

			// LightingMainBuffer2D.Clear();
			// Light2D.ForceUpdateAll();
		}
		
		EditorGUILayout.Space();

		int count = script.cameraSettings.Length;
		count = EditorGUILayout.IntSlider("Camera Count", count, 0, 10);
		if (count != script.cameraSettings.Length) {
			System.Array.Resize(ref script.cameraSettings, count);
		}

		EditorGUILayout.Space();

		for(int id = 0; id < script.cameraSettings.Length; id++) {
			CameraSettings cameraSetting = script.cameraSettings[id];

			cameraFoldout[id] = EditorGUILayout.Foldout(cameraFoldout[id], "Camera " + (id + 1) + " (" + cameraSetting.GetTypeName() + ")");

			if (cameraFoldout[id] == false) {
				EditorGUILayout.Space();
				continue;
			}

			EditorGUI.indentLevel++;

			cameraSetting.cameraType = (CameraSettings.CameraType)EditorGUILayout.EnumPopup("Camera Type", cameraSetting.cameraType);

			if (cameraSetting.cameraType == CameraSettings.CameraType.Custom) {
				cameraSetting.customCamera = (Camera)EditorGUILayout.ObjectField(cameraSetting.customCamera, typeof(Camera), true);
			}

			cameraSetting.bufferID = EditorGUILayout.Popup("Buffer Preset", (int)cameraSetting.bufferID, Lighting2D.Profile.bufferPresets.GetBufferLayers());

			cameraSetting.renderMode = (CameraSettings.RenderMode)EditorGUILayout.EnumPopup("Render Mode", cameraSetting.renderMode);

			if (cameraSetting.renderMode == CameraSettings.RenderMode.Draw) {
				cameraSetting.renderShader = (CameraSettings.RenderShader)EditorGUILayout.EnumPopup("Render Shader", cameraSetting.renderShader);
			
				if (cameraSetting.renderShader == CameraSettings.RenderShader.Custom) {
					cameraSetting.customMaterial = (Material)EditorGUILayout.ObjectField(cameraSetting.customMaterial, typeof(Material), true);
				}

				cameraSetting.renderLayerType = (CameraSettings.RenderLayerType)EditorGUILayout.EnumPopup("Render Layer Type", cameraSetting.renderLayerType);

				if (cameraSetting.renderLayerType == CameraSettings.RenderLayerType.Custom) {
					cameraSetting.renderLayerId = EditorGUILayout.LayerField("Render Layer", cameraSetting.renderLayerId);
				}

			}

			cameraSetting.id = id;

			script.cameraSettings[id] = cameraSetting;

			EditorGUI.indentLevel--;

			EditorGUILayout.Space();
		}

		fogOfWarFoldout = EditorGUILayout.Foldout(fogOfWarFoldout, "Fog Of War (RT)");

		if (fogOfWarFoldout) {
			EditorGUI.indentLevel++;

			EditorGUILayout.Space();

			count = script.fogOfWarCameras.Length;
			count = EditorGUILayout.IntSlider("Camera Count", count, 0, 10);
			if (count != script.fogOfWarCameras.Length) {
				System.Array.Resize(ref script.fogOfWarCameras, count);
			}

			EditorGUILayout.Space();
	
			for(int id = 0; id < script.fogOfWarCameras.Length; id++) {
				FogOfWarCamera fogOfWarCamera = script.fogOfWarCameras[id];

				fogOfWarCameraFoldout[id] = EditorGUILayout.Foldout(fogOfWarCameraFoldout[id], "Camera " + (id + 1) + " (" + fogOfWarCamera.GetTypeName() + ")");

				if (fogOfWarCameraFoldout[id] == false) {
					EditorGUILayout.Space();
					continue;
				}

				EditorGUI.indentLevel++;

				fogOfWarCamera.cameraType = (FogOfWarCamera.CameraType)EditorGUILayout.EnumPopup("Camera Type", fogOfWarCamera.cameraType);

				if (fogOfWarCamera.cameraType == FogOfWarCamera.CameraType.Custom) {
					fogOfWarCamera.customCamera = (Camera)EditorGUILayout.ObjectField(fogOfWarCamera.customCamera, typeof(Camera), true);
				}

				fogOfWarCamera.bufferID = EditorGUILayout.Popup("Buffer Preset", (int)fogOfWarCamera.bufferID, Lighting2D.Profile.bufferPresets.GetBufferLayers());


				fogOfWarCamera.renderLayerType = (FogOfWarCamera.RenderLayerType)EditorGUILayout.EnumPopup("Render Layer Type", fogOfWarCamera.renderLayerType);

				if (fogOfWarCamera.renderLayerType == FogOfWarCamera.RenderLayerType.Custom) {
					fogOfWarCamera.renderLayerId = EditorGUILayout.LayerField("Render Layer", fogOfWarCamera.renderLayerId);
				}


				fogOfWarCamera.id = id;

				script.fogOfWarCameras[id] = fogOfWarCamera;

				EditorGUI.indentLevel--;

				EditorGUILayout.Space();
			}



			EditorGUI.indentLevel--;
		}


	/*
		

	cameraSetting.renderMode = (CameraSettings.RenderMode)EditorGUILayout.EnumPopup("Render Mode", cameraSetting.renderMode);

	if (cameraSetting.renderMode == CameraSettings.RenderMode.Draw) {
		cameraSetting.renderShader = (CameraSettings.RenderShader)EditorGUILayout.EnumPopup("Render Shader", cameraSetting.renderShader);
	
		if (cameraSetting.renderShader == CameraSettings.RenderShader.Custom) {
		cameraSetting.customMaterial = (Material)EditorGUILayout.ObjectField(cameraSetting.customMaterial, typeof(Material), true);
		}
	}*/

		

		EditorGUILayout.Space();

		EditorGUILayout.LabelField("version " + Lighting2D.VERSION_STRING);

		string buttonName = "";
		if (script.version < Lighting2D.VERSION) {
			buttonName += "Re-Initialize (Outdated)";
			GUI.backgroundColor = Color.red;

			Reinitialize(script);

			return;
		} else {
			buttonName += "Re-Initialize";
		}
		
		if (GUILayout.Button(buttonName)) {
			Reinitialize(script);
		}

		if (GUI.changed) {
			Light2D.ForceUpdateAll();

			LightingManager2D.ForceUpdate();

			if (EditorApplication.isPlaying == false) {
				
				EditorUtility.SetDirty(target);
				EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
				
			}
		}
	}

	public void Reinitialize(LightingManager2D manager) {
		Debug.Log("Lighting Manager 2D: reinitialized");

		if (manager.version > 0 && manager.version < Lighting2D.VERSION) {
			Debug.Log("Lighting Manager 2D: version update from " + manager.version + " to " + Lighting2D.VERSION);
		}

		foreach(Transform transform in manager.transform) {
			DestroyImmediate(transform.gameObject);
		}
			
		manager.Initialize();

		Light2D.ForceUpdateAll();

		LightingManager2D.ForceUpdate();

		if (EditorApplication.isPlaying == false) {
			
			EditorUtility.SetDirty(target);
			EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
			
		}
	}
}