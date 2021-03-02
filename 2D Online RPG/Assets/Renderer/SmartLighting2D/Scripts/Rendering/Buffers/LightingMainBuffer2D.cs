using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LightingSettings;

[ExecuteInEditMode] 
public class LightingMainBuffer2D {
	public string name = "Uknown";

	private LightingMaterial material = null;

	public bool updateNeeded = false;

	public LightTexture renderTexture;
	public CameraSettings cameraSettings;

	public static List<LightingMainBuffer2D> List = new List<LightingMainBuffer2D>();

	public LightingMainBuffer2D() {
		List.Add(this);
	}

	public static void Clear() {
		foreach(LightingMainBuffer2D buffer in new List<LightingMainBuffer2D>(List)) {
			buffer.DestroySelf();
		}

		List.Clear();
	}

	public void DestroySelf() {
		if (renderTexture != null) {
			if (renderTexture.renderTexture != null) {
				if (Application.isPlaying) {
					UnityEngine.Object.Destroy (renderTexture.renderTexture);
				} else {
					UnityEngine.Object.DestroyImmediate (renderTexture.renderTexture);
				}
			}
		}

		List.Remove(this);
	}

	public bool IsActive() {
		return(List.IndexOf(this) > -1);
	}

	static public LightingMainBuffer2D Get(FogOfWarCamera fogOfWarCamera) {
		if (fogOfWarCamera.GetCamera() == null) {
			return(null);
		}

		foreach(LightingMainBuffer2D mainBuffer in List) {
			if (mainBuffer.cameraSettings.GetCamera() == fogOfWarCamera.GetCamera() && mainBuffer.cameraSettings.bufferID == fogOfWarCamera.bufferID) {
				return(mainBuffer);
			}
		}


		return(null);
	}

	static public LightingMainBuffer2D Get(CameraSettings cameraSettings) {
		if (cameraSettings.GetCamera() == null) {
			return(null);
		}

		foreach(LightingMainBuffer2D mainBuffer in List) {
			if (mainBuffer.cameraSettings.GetCamera() == cameraSettings.GetCamera() && mainBuffer.cameraSettings.bufferID == cameraSettings.bufferID) {
				return(mainBuffer);
			}
		}

		if (Lighting2D.BufferPresets.Length <= cameraSettings.bufferID) {
			Debug.LogWarning("Lighting2D: Not enough buffer settings initialized");

			return(null);
		}

		Buffers.Get();

		LightingMainBuffer2D buffer = new LightingMainBuffer2D();
		buffer.cameraSettings = cameraSettings;

		Rendering.LightingMainBuffer.InitializeRenderTexture(buffer);

		return(buffer);
	}

	public BufferPreset GetBufferPreset() {
		if (Lighting2D.BufferPresets.Length <= cameraSettings.bufferID) {
			Debug.LogWarning("Lighting2D: Not enough buffer settings initialized");

			return(null);
		}

		return(Lighting2D.BufferPresets[cameraSettings.bufferID]);
	}

	public void ClearMaterial() {
		material = null;
	}

	public Material GetMaterial() {
		if (material == null || material.Get() == null) {
			switch(cameraSettings.renderShader) {
				case CameraSettings.RenderShader.MultiplyHDR:

					material = LightingMaterial.Load("Light2D/Internal/Multiply HDR");

				break;
				
				case CameraSettings.RenderShader.Multiply:
				
					material = LightingMaterial.Load("Light2D/Internal/Multiply");
					
				break;

				case CameraSettings.RenderShader.Additive:
					
					material = LightingMaterial.Load(Max2D.shaderPath + "Particles/Additive");
					
				break;

				case CameraSettings.RenderShader.Custom:

					material = LightingMaterial.Load(cameraSettings.GetMaterial());

				break;
			}
		}

		material.SetTexture(renderTexture.renderTexture);

		return(material.Get());
	}

	public void Update() {
		Rendering.LightingMainBuffer.Update(this);
	}

	public void Render() {
		if (cameraSettings.renderMode == CameraSettings.RenderMode.Disabled) {
			return;
		}

		if (updateNeeded) {

			Camera camera = Camera.current;
			if (camera != null) {
				// return;	
			}
		
			RenderTexture previous = RenderTexture.active;

			RenderTexture.active = renderTexture.renderTexture;
			GL.Clear(true, true, Color.white); // Clear Darkness Color

			Rendering.LightingMainBuffer.Render(this);

			RenderTexture.active = previous;
		}


		Rendering.LightingMainBuffer.DrawOn(this);
	}

	// Apply Render to Specified Camera (Post Render Mode)
	public void OnRenderObject() {
		if (Lighting2D.disable) {
			return;
		}
		
		
	}
}