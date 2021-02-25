using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LightingSettings;

#if UNITY_EDITOR
    using UnityEditor;
#endif

[ExecuteInEditMode]
public class OnRenderMode : LightingMonoBehaviour {
    public LightingMainBuffer2D mainBuffer;
    public MeshRenderer meshRenderer;
    public MeshFilter meshFilter;

    public static List<OnRenderMode> List = new List<OnRenderMode>();

	public void OnEnable() {
		List.Add(this);
	}

	public void OnDisable() {
		List.Remove(this);   
    }

    public static OnRenderMode Get(LightingMainBuffer2D buffer) {
		foreach(OnRenderMode meshModeObject in List) {
			if (meshModeObject.mainBuffer == buffer) {
                return(meshModeObject);
            }
		}

        GameObject meshRendererMode = new GameObject("On Render");
        OnRenderMode onRenderMode = meshRendererMode.AddComponent<OnRenderMode>();

        onRenderMode.mainBuffer = buffer;
        onRenderMode.Initialize(buffer);
        onRenderMode.UpdateLayer();

        if (Lighting2D.ProjectSettings.managerInternal == LightingSettings.ManagerInternal.HideInHierarchy) {
            meshRendererMode.hideFlags = meshRendererMode.hideFlags | HideFlags.HideInHierarchy;
        }

        onRenderMode.name = "On Render: " + buffer.name;

        return(onRenderMode);
    }

    public void Initialize(LightingMainBuffer2D mainBuffer) {
        gameObject.transform.parent = Buffers.Get().transform;
        
        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = mainBuffer.GetMaterial();

        BufferPreset bufferPreset = mainBuffer.GetBufferPreset();
           
        bufferPreset.sortingLayer.ApplyToMeshRenderer(meshRenderer);

        // Disable Mesh Renderer Settings
        meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        meshRenderer.receiveShadows = false;
        meshRenderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
        meshRenderer.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
        meshRenderer.allowOcclusionWhenDynamic = false;

        UpdatePosition();

        meshFilter = gameObject.AddComponent<MeshFilter>();
        meshFilter.mesh = LightingRender2D.GetMesh();
    }

    void Update() {
        if (mainBuffer == null || mainBuffer.IsActive() == false) {
            DestroySelf();
            return;
        }

        if (mainBuffer.cameraSettings.GetCamera() == null) {
            DestroySelf();
            return;
        }

        if (Lighting2D.RenderingMode != RenderingMode.OnRender) {
            DestroySelf();
            return;
        }
    }

    public void UpdateLoop() {
        if (mainBuffer == null || mainBuffer.IsActive() == false) {
            return;
        }

        if (mainBuffer.cameraSettings.GetCamera() == null) {
            return;
        }

        if (Lighting2D.RenderingMode != RenderingMode.OnRender) {
            return;
        }

        UpdateLayer();

        if (Lighting2D.disable) {
            if (meshRenderer != null) {
				meshRenderer.enabled = false;
			}
        }
		
        if (mainBuffer.cameraSettings.renderMode != CameraSettings.RenderMode.Draw) {
			meshRenderer.enabled = false;
		}
      
		if (Lighting2D.RenderingMode == RenderingMode.OnRender) {
            UpdatePosition();
        }
    }

    void UpdateLayer() {
        int layer = 0;

        if (mainBuffer != null) {
            layer = mainBuffer.cameraSettings.GetLayerId();
        }

        gameObject.layer = layer;
    }

    public void UpdatePosition() {
        
        Camera camera = mainBuffer.cameraSettings.GetCamera();
        
        if (camera == null) {
            return;
        }

        
        transform.position = LightingPosition.GetCameraPlanePosition(camera);
        transform.rotation = camera.transform.rotation;

        // Local scale, not great
        transform.localScale = LightingRender2D.GetSize(camera);

        // transform.localScale = LightingRender2D.GetSize(camera);
    }
}