using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LightingSettings;

public class FogOfWarRender {

    public static bool IsReady(FogOfWarBuffer2D buffer) {
        if (buffer == null) {
            return(false);
        }	

        return(true);
    }

    public static void OnRender(FogOfWarBuffer2D buffer) {
        Camera camera = buffer.fogOfWarCamera.GetCamera();

        if (camera == null) {
            return;
        }

        if (IsReady(buffer) == false) {
            return;
        }	

        FogOfWarOnRenderMode onRenderMode = FogOfWarOnRenderMode.Get(buffer);
        if (onRenderMode == null) {
            return;
        }

        onRenderMode.UpdatePosition();

        if (onRenderMode.meshRenderer != null) {	
            onRenderMode.meshRenderer.enabled = true;
            if (onRenderMode.meshRenderer.sharedMaterial != buffer.GetMaterial()) {
                onRenderMode.meshRenderer.sharedMaterial = buffer.GetMaterial();
            }
            
            if (onRenderMode.meshRenderer.sharedMaterial == null) {
                onRenderMode.meshRenderer.sharedMaterial = buffer.GetMaterial();
            }
        }
    }

    // Post-Render Mode Drawing
    public static void PostRender(FogOfWarBuffer2D buffer) {
        Camera camera = buffer.fogOfWarCamera.GetCamera();

        if (IsReady(buffer) == false) {
            return;
        }

        if (Lighting2D.RenderingMode != RenderingMode.OnPostRender) {
            return;
        }

        if (Camera.current != camera) {
            return;
        }

        Rendering.Universal.Texture.Draw(buffer.GetMaterial(), LightingPosition.GetCameraPlanePosition(camera), LightingRender2D.GetSize(camera), camera.transform.eulerAngles.z, LightingPosition.GetCameraPlanePosition(camera).z);
    }
    
    // Graphics.Draw() Mode Drawing
    static public void PreRender(FogOfWarBuffer2D buffer) {
        Camera camera = buffer.fogOfWarCamera.GetCamera();

        if (IsReady(buffer) == false) {
            return;
        }

        Graphics.DrawMesh(LightingRender2D.GetMesh(), Matrix4x4.TRS(LightingPosition.GetCameraPlanePosition(camera), camera.transform.rotation, LightingRender2D.GetSize(camera)), buffer.GetMaterial(), 0, camera);
    }
}
