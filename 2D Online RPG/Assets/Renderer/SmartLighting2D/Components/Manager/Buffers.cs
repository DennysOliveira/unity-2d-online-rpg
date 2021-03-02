using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Buffers : MonoBehaviour {
    static private Buffers instance;

	bool renderObject = false;

	public void Awake() {
		UpdateFlags();
	}

	private void OnDestroy() {
		instance = null;
	}

    static public Buffers Get() {
		if (instance != null) {
			if (instance.transform != null) {
				return(instance);
			} else {
				instance = null;
			}			
		}

		foreach(Buffers root in Object.FindObjectsOfType(typeof(Buffers))) {
			instance = root;

			return(instance);
		}


		LightingManager2D manager = LightingManager2D.Get();

		GameObject gameObject = new GameObject ();
		gameObject.transform.parent = manager.transform;
		gameObject.name = "Buffers";

		instance = gameObject.AddComponent<Buffers> ();
		instance.GetCamera();
		instance.UpdateFlags();

		return(instance);
	}

	void UpdateFlags() {
		if (Lighting2D.ProjectSettings.managerInternal == LightingSettings.ManagerInternal.HideInHierarchy) {
			gameObject.hideFlags = HideFlags.HideInHierarchy;
		} else {
			gameObject.hideFlags = HideFlags.None;
		}
	}

	private Camera lightingCamera = null;

	private void OnEnable() {
		SetUpCamera();
	}

	public Camera GetCamera() {
		if (lightingCamera == null) {
			lightingCamera = gameObject.GetComponent<Camera>();

			if (lightingCamera == null) {
				lightingCamera = gameObject.AddComponent<Camera>();
				SetUpCamera();
			}
		}

		return(lightingCamera);
	}

	#if UNITY_EDITOR
		private void Update() {
			LightingManager2D manager = LightingManager2D.Get();

			if (manager != null) {
				gameObject.layer = Lighting2D.ProjectSettings.editorView.gameViewLayer;
			}
		}
	#endif

	private void LateUpdate() {
		renderObject = true;
	}

    private void OnPreCull() {
		if (Lighting2D.disable) {
			return;
		}

        if (Lighting2D.Profile.qualitySettings.updateMethod != LightingSettings.UpdateMethod.OnPreCull) {
			return;
        }
		
		LightingManager2D manager = LightingManager2D.Get();
		manager.RenderLoop();
    }

	private void OnRenderObject() {
		if (Lighting2D.disable) {
			return;
		}
		
        if (Lighting2D.Profile.qualitySettings.updateMethod != LightingSettings.UpdateMethod.OnRenderObject) {
			return;
        }

		if (renderObject == false) {
			return;
		}

		renderObject = false;

		LightingManager2D manager = LightingManager2D.Get();
		manager.RenderLoop();
    }

	void SetUpCamera() {
		if (lightingCamera == null) {
			return;
		}

		lightingCamera.clearFlags = CameraClearFlags.Nothing;
		lightingCamera.backgroundColor = Color.white;
		lightingCamera.cameraType = CameraType.Game;
		lightingCamera.orthographic = true;
		lightingCamera.farClipPlane = 0.01f;
		lightingCamera.nearClipPlane = -0.01f;
		lightingCamera.allowHDR = false;
		lightingCamera.allowMSAA = false;
		lightingCamera.enabled = false;
		lightingCamera.depth = -50;

		if (Camera.main != null) {
			lightingCamera.orthographicSize = Camera.main.orthographicSize;
		}
		
	}
























	
    // Management
	static public LightingBuffer2D AddBuffer(Vector2Int textureSize, Light2D light) { // i 
		Get();

		LightingBuffer2D lightingBuffer2D = new LightingBuffer2D ();
		lightingBuffer2D.Initiate (textureSize);
		lightingBuffer2D.Light = light; // Unnecessary?

		return(lightingBuffer2D);
	}

	static public LightingBuffer2D PullBuffer(Vector2Int textureSize, Light2D light) {
		Get();

		foreach (LightingBuffer2D id in LightingBuffer2D.List) {
			if (id.Free && id.renderTexture.width == textureSize.x && id.renderTexture.height == textureSize.y) {
				id.Light = light;

				light.ForceUpdate();
				
				return(id);
			}
		}
			
		return(AddBuffer(textureSize, light));		
	}

    static public void FreeBuffer(LightingBuffer2D buffer) {
        if (buffer == null) {
            return;
        }

		if (buffer.Light != null) {
			buffer.Light.Buffer = null;

			buffer.Light = null;
		}

		buffer.updateNeeded = false;
	}
}
