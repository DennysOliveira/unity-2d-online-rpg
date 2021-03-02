using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LightingSettings;

[ExecuteInEditMode] 
public class FogOfWarOnRenderMode : LightingMonoBehaviour {
    public FogOfWarBuffer2D buffer;

    public MeshRenderer meshRenderer;
    public MeshFilter meshFilter;

    public static List<FogOfWarOnRenderMode> list = new List<FogOfWarOnRenderMode>();

	public void OnEnable() {
		list.Add(this);

        Lighting2D.FogOfWar.sortingLayer.ApplyToMeshRenderer(GetMeshRenderer());
	}

	public void OnDisable() {
		list.Remove(this);
	}

    private void OnDestroy() {
       list.Remove(this); 
    }

    public MeshRenderer GetMeshRenderer() {
        if (meshRenderer == null) {
            meshRenderer = gameObject.AddComponent<MeshRenderer>();
        }
        return(meshRenderer);
    }

    public static FogOfWarOnRenderMode Get(FogOfWarBuffer2D mainBuffer) {
		foreach(FogOfWarOnRenderMode meshModeObject in list) {
			if (meshModeObject.buffer == mainBuffer) {
                return(meshModeObject);
            }
		}

        GameObject meshRendererMode = new GameObject("On Render: " + mainBuffer.name);
        FogOfWarOnRenderMode onRenderMode = meshRendererMode.AddComponent<FogOfWarOnRenderMode>();

        if (Lighting2D.ProjectSettings.managerInternal == LightingSettings.ManagerInternal.HideInHierarchy) {
            meshRendererMode.hideFlags = meshRendererMode.hideFlags | HideFlags.HideInHierarchy;
        }

        onRenderMode.buffer = mainBuffer;
        onRenderMode.Initialize(mainBuffer);

        return(onRenderMode);
    }

    public void Initialize(FogOfWarBuffer2D mainBuffer) {         
        transform.parent = Buffers.Get().transform;
        
        GetMeshRenderer();
        meshRenderer.sharedMaterial = new Material(Shader.Find("Light2D/Internal/AlphaMask"));
           
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

    public void UpdatePosition() {
        if (buffer == null){
            return;
        }

        Camera camera = buffer.fogOfWarCamera.GetCamera();
        if (camera == null) {
            return;
        }
        
        Vector3 position = camera.transform.position;
        position.z += camera.nearClipPlane + 0.2f;

        transform.position = position;
        transform.rotation = camera.transform.rotation;
        transform.localScale = LightingRender2D.GetSize(camera);
    }
    
    public void Update() {
        if (buffer == null) {
            DestroySelf();
            return;
        }

        Camera camera = buffer.fogOfWarCamera.GetCamera();
        if (camera == null) {
            return;
        }
        
        if (camera == null) {
            DestroySelf();
        }

        if (Lighting2D.RenderingMode != RenderingMode.OnRender) {
            DestroySelf();

            return;
		}

        LightingManager2D manager = LightingManager2D.Get();

        int layer = 0;

        if (buffer != null) {
            layer = buffer.fogOfWarCamera.GetLayerId();
        }

        gameObject.layer = layer;

        if (Lighting2D.disable) {
            if (meshRenderer != null) {
				meshRenderer.enabled = false;
			}
        }
        
        if (manager.fogOfWarCameras.Length < 1) {
			meshRenderer.enabled = false;
		}
			
		if (Lighting2D.RenderingMode != RenderingMode.OnRender) {
			meshRenderer.enabled = false;
		}
    }

    void LateUpdate() {
		if (Lighting2D.RenderingMode == RenderingMode.OnRender) {
            UpdatePosition();
        }

        if (buffer == null || buffer.IsActive() == false) {
            DestroySelf();
            return;
        }

        if (buffer.fogOfWarCamera.GetCamera() == null) {
            DestroySelf();

            return;
        }

        //if (Lighting2D.RenderingMode != RenderingMode.OnRender) {
        //   DestroySelf();
        //    return;
        //}
    }
}
