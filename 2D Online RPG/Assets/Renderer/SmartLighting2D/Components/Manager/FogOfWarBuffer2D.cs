using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LightingSettings;

[ExecuteInEditMode] 
public class FogOfWarBuffer2D {

    public static List<FogOfWarBuffer2D> List = new List<FogOfWarBuffer2D>();

	public string name = "";

    public LightTexture renderTexture;
	public FogOfWarCamera fogOfWarCamera;

    LightingMaterial material = null;

	public static void Clear() {
		foreach(FogOfWarBuffer2D buffer in new List<FogOfWarBuffer2D>(List)) {
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

    public Material GetMaterial() {
		if (material == null || material.Get() == null) {
            material = LightingMaterial.Load("Light2D/Internal/AlphaMask");
		}

		LightingMainBuffer2D buffer = LightingMainBuffer2D.Get(fogOfWarCamera);

		if (buffer != null) {
			Texture textureAlpha = buffer.renderTexture.renderTexture;

			Material mat = material.Get();

			if (renderTexture != null) {
				mat.mainTexture = renderTexture.renderTexture;  
			}

			mat.SetTexture("_Mask", textureAlpha);
		}

		if (Lighting2D.ProjectSettings.colorSpace == LightingSettings.ColorSpace.Linear) {
			material.Get().SetFloat("_LinearColor", 1);
		} else {
			material.Get().SetFloat("_ColorSpace", 0);
		}

		return(material.Get());
	}

    static public FogOfWarBuffer2D Get(FogOfWarCamera fogOfWarCamera) {
        foreach(FogOfWarBuffer2D FoWBuffer in List) {
            if (FoWBuffer.fogOfWarCamera.GetCamera() == fogOfWarCamera.GetCamera() && FoWBuffer.fogOfWarCamera.bufferID == fogOfWarCamera.bufferID) {
                return(FoWBuffer);
            }
        }

        FogOfWarBuffer2D buffer = new FogOfWarBuffer2D();
        buffer.fogOfWarCamera = fogOfWarCamera;
        buffer.SetUpRenderTexture ();

		List.Add(buffer);
        
        return(buffer);
    }

	public void SetUpRenderTexture() {
		Vector2Int screen = GetScreen();

		if (screen.x > 0 && screen.y > 0) {
			string idName = "?";
			
			int bufferId = fogOfWarCamera.bufferID;

			Camera camera = fogOfWarCamera.GetCamera();

			name = "Fog of War Buffer (" + idName +", Id: " + (bufferId  + 1) + ", Camera: " + camera.name + " )";

			renderTexture = new LightTexture (screen.x, screen.y, 32);
			renderTexture.renderTexture.filterMode = Lighting2D.Profile.fogOfWar.filterMode;
			renderTexture.Create ();
		}
	}

	public Vector2Int GetScreen() {
		Camera camera = fogOfWarCamera.GetCamera();

		if (camera == null) {
			return(Vector2Int.zero);
		}

		int screenWidth = (int)(camera.pixelRect.width * Lighting2D.FogOfWar.resolution);
		int screenHeight = (int)(camera.pixelRect.height * Lighting2D.FogOfWar.resolution);

		Vector2Int screen = new Vector2Int(screenWidth, screenHeight);

		return(screen);
	}

	public bool CameraSettingsCheck () {
		LightingManager2D manager = LightingManager2D.Get();
		int settingsID = fogOfWarCamera.id;

		if (settingsID >= manager.fogOfWarCameras.Length) {
			return(false);
		}

		if (manager.fogOfWarCameras[settingsID].Equals(fogOfWarCamera) == false) {
			return(false);
		}

		if (fogOfWarCamera.GetCamera() == null) {
			return(false);
		}

		//if (fogOfWarCamera.bufferID != fogOfWarCamera.bufferID) {
		//	return(false);
		//}

		//cameraSettings.renderMode = manager.cameraSettings[settingsID].renderMode;

		return(true);
	}

    public void UpdateLoop() {
		if (CameraSettingsCheck() == false) {
			DestroySelf();
			return;
		}
		//Debug.Log(list.IndexOf(this));//
		
		Rendering.FogOfWarBuffer.Check.RenderTexture(this);

		Rendering.FogOfWarBuffer.LateUpdate(this);
    }

	public void Render() {
		if (renderTexture != null) {
			RenderTexture previous = RenderTexture.active;

			RenderTexture.active = renderTexture.renderTexture;
			GL.Clear( false, true, new Color(0, 0, 0, 0));

			Rendering.FogOfWarBuffer.Render(this);

			RenderTexture.active = previous;
		}

		Rendering.FogOfWarBuffer.DrawOn(this);
	}

	// Apply Render to Specified Camera (Post Render Mode)
	private void OnRenderObject() {
		if (Lighting2D.disable) {
			return;
		}

		if (Lighting2D.RenderingMode != RenderingMode.OnPostRender) {
			return;
		}

		// if (cameraSettings.renderMode != CameraSettings.RenderMode.Draw) {
		// 	return;
		// }

		FogOfWarRender.PostRender(this);
	}
}