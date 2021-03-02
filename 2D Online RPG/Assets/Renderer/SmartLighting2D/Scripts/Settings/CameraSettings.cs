using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
    using UnityEditor;
#endif

[System.Serializable]
public struct CameraSettings {
	public int id;

	public enum RenderMode { Draw, Hidden, Disabled }
	public enum CameraType { MainCamera, Custom, SceneView };
	public enum RenderShader { MultiplyHDR, Multiply, Additive, Custom };
	public enum RenderLayerType { Default, Custom };
	
	public RenderLayerType renderLayerType;
	public CameraType cameraType;
	public Camera customCamera;
	public RenderMode renderMode;
	public RenderShader renderShader;
	public Material customMaterial;

	public Material customMaterialInstance;

	public Material GetMaterial() {
		if (customMaterialInstance == null) {
			customMaterialInstance = new Material(customMaterial);
		}

		return(customMaterialInstance);
	}

	public int bufferID;

	public int renderLayerId;

	public string GetTypeName() {
		switch(cameraType) {
			case CameraType.SceneView:
				return("Scene View");

			case CameraType.MainCamera:
				return("Main Camera Tag");

			case CameraType.Custom:
				return("Custom");

			default:
				return("Unknown");
		}
	}

	public int GetLayerId() {
		if (renderLayerType == RenderLayerType.Custom) {
			return(renderLayerId);
		} else {
			Camera camera = GetCamera();

			if (camera != null && cameraType == CameraType.SceneView) {
				return(Lighting2D.ProjectSettings.editorView.sceneViewLayer);
			} else {
				return(Lighting2D.ProjectSettings.editorView.gameViewLayer);
			}
		}
	}

	public CameraSettings(int id = 0) {
		this.id = id;

		renderMode = RenderMode.Draw;

		cameraType = CameraType.MainCamera;

		renderShader = RenderShader.Multiply;

		renderLayerType = RenderLayerType.Default;

		customMaterial = null;

		customMaterialInstance = null;

		bufferID = 0;

		renderLayerId = 0;

		customCamera = null;
	}

	public Camera GetCamera() {
		Camera camera = null;
		switch(cameraType) {
			case CameraType.MainCamera:
				camera = Camera.main;

				if (camera != null) {
					if (camera.orthographic == false) {
						return(null);
					}
				}

				return(Camera.main);

			case CameraType.Custom:
				camera = customCamera;

				if (camera != null) {
					if (camera.orthographic == false) {
						return(null);
					}
				}

				return(customCamera);


            case CameraType.SceneView:
			
				#if UNITY_EDITOR
					SceneView sceneView = SceneView.lastActiveSceneView;

					if (sceneView != null) {
						camera = sceneView.camera;

						#if UNITY_2019_1_OR_NEWER
						
							if (SceneView.lastActiveSceneView.sceneLighting == false) {
								camera = null;
							}

						#else
						
							if (SceneView.lastActiveSceneView.m_SceneLighting == false) {
								camera = null;
							}

						#endif
					}
	
					if (camera != null && camera.orthographic == false) {
						camera = null;
					}

					if (camera != null) {
						if (camera.orthographic == false) {
							return(null);
						}
					}

					return(camera);

				#else
					return(null);

				#endif
				
		}

		return(null);
	}

	public bool Equals(CameraSettings obj) {
        return this.bufferID == obj.bufferID && this.customCamera == obj.customCamera && this.cameraType == obj.cameraType;
    }

	public override int GetHashCode() {
        return this.GetHashCode();
    }
}