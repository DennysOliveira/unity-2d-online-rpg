using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
    using UnityEditor;
#endif

[System.Serializable]
public struct FogOfWarCamera {
	public int id;

	public enum CameraType {MainCamera, Custom, SceneView};
	public enum RenderLayerType { Default, Custom };

	public RenderLayerType renderLayerType;
	public CameraType cameraType;
	public Camera customCamera;
	//public RenderMode renderMode;

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

	public FogOfWarCamera(int id = 0) {
		this.id = id;

		//renderMode = RenderMode.Draw;

		cameraType = CameraType.MainCamera;

		renderLayerType = RenderLayerType.Default;

		renderLayerId = 0;

		bufferID = 0;

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

	public bool Equals(FogOfWarCamera obj) {
        return this.bufferID == obj.bufferID && this.customCamera == obj.customCamera && this.cameraType == obj.cameraType;
    }

	public override int GetHashCode() {
        return this.GetHashCode();
    }
}