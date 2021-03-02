using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using LightingSettings;

[ExecuteInEditMode]
public class Light2D : LightingMonoBehaviour {
	public enum LightSprite {Default, Custom};
	public enum WhenInsideCollider {DrawInside = 1, DrawAbove = 0}; // Draw Bellow / Do Not Draw
	public enum LitMode {Everything, MaskOnly}

	// Settings
	public int lightPresetId = 0;
	public int eventPresetId = 0;
	
	public int nightLayer = 0;

	public Color color = new Color(.5f, .5f, .5f, 1);

	public float size = 5f;
	public float coreSize = 0.5f;
	public float spotAngle = 360;
	public float outerAngle = 15;
	public float maskTranslucency = 0;
	public float maskTranslucencyIntensity = 1;
	public LitMode litMode = LitMode.Everything;
	
	public bool applyRotation = false;

	public LightingSourceTextureSize textureSize = LightingSourceTextureSize.px2048;

	public MeshMode meshMode = new MeshMode();
	public BumpMap bumpMap = new BumpMap();

	public WhenInsideCollider whenInsideCollider = WhenInsideCollider.DrawInside;

	public LightSprite lightSprite = LightSprite.Default;
	public Sprite sprite;
	public bool spriteFlipX = false;
	public bool spriteFlipY = false;
	
	public LightTransform transform2D;
	public LightEventHandling eventHandling = new LightEventHandling();

	[System.Serializable]
	public class LightEventHandling {
		public EventHandling.Object eventHandlingObject = new EventHandling.Object();
	}

	// Internal
	private List<LightCollider2D> collidersInside = new List<LightCollider2D>();
	private List<LightCollider2D> collidersInsideRemove = new List<LightCollider2D>();

	public static List<Light2D> List = new List<Light2D>();	
	private bool inScreen = false;
	private LightingBuffer2D buffer = null;
	private static Sprite defaultSprite = null;

	public LightingBuffer2D Buffer {
		get => buffer;
		set => buffer = value;
	}

	public void AddCollider(LightCollider2D id) {
		if (collidersInside.Contains(id)) {
			if (lightPresetId > 0) {
				if (id.lightOnEnter != null) {
					id.lightOnEnter.Invoke(this);
				}
			}
			
			collidersInside.Add(id);
		}
	}

	[System.Serializable]
	public class BumpMap {
		public float intensity = 1;
		public float depth = 1;
	}

	public Rect GetWorldRect() {
		return(new Rect(transform2D.position.x - size, transform2D.position.y - size, size * 2, size * 2));
	}

	public LayerSetting[] GetLayerSettings() {
		LightPresetList presetList = Lighting2D.Profile.lightPresets;
		
		if (lightPresetId >= presetList.list.Length) {
			return(null);
		}

		LightPreset lightPreset = presetList.Get()[lightPresetId];

		return(lightPreset.layerSetting.Get());
	}

	public EventPreset GetEventPreset() {
		EventPresetList presetList = Lighting2D.Profile.eventPresets;
		
		if (eventPresetId >= presetList.list.Length) {
			return(null);
		}

		EventPreset lightPreset = presetList.Get()[eventPresetId];

		return(lightPreset);
	}

	static public Sprite GetDefaultSprite() {
		if (defaultSprite == null || defaultSprite.texture == null) {
			defaultSprite = Resources.Load <Sprite> ("Sprites/gfx_light");
		}
		return(defaultSprite);
	}

	public Sprite GetSprite() {
		if (sprite == null || sprite.texture == null) {
			sprite = GetDefaultSprite();
		}
		return(sprite);
	}

	public void ForceUpdate() {
		transform2D.ForceUpdate();
	}

	static public void ForceUpdateAll() {
		foreach(Light2D light in Light2D.List) {
			light.ForceUpdate();
		}
	}

	public void OnEnable() {
		List.Add(this);

		if (transform2D == null) {
			transform2D = new LightTransform();
		}

		LightingManager2D.Get();

		collidersInside.Clear();

		transform2D.ForceUpdate();

		ForceUpdate();
	}

	public void OnDisable() {
		List.Remove(this);

		Free();
	}

	public void Free() {
		Buffers.FreeBuffer(buffer);

		inScreen = false;
	}

	public bool InAnyCamera() {
		LightingManager2D manager = LightingManager2D.Get();
		CameraSettings[] cameraSettings = manager.cameraSettings;

		Rect lightRect = GetWorldRect();

		for(int i = 0; i < cameraSettings.Length; i++) {
			Camera camera = manager.GetCamera(i);

			if (camera == null) {
				continue;
			}

			Rect cameraRect = CameraTransform.GetWorldRect(camera);

			if (cameraRect.Overlaps(lightRect)) {
				return(true);
			}
		}

		return(false);
	}

	public Vector2Int GetTextureSize() {
		Vector2Int textureSize2D = LightingRender2D.GetTextureSize(textureSize);

		if (Lighting2D.Profile.qualitySettings.lightTextureSize != LightingSettings.LightingSourceTextureSize.Custom) {
            textureSize2D = LightingRender2D.GetTextureSize(Lighting2D.Profile.qualitySettings.lightTextureSize);
        }

		return(textureSize2D);
	}
	
	public bool IsPixelPerfect() {
		if (Lighting2D.Profile.qualitySettings.lightTextureSize != LightingSettings.LightingSourceTextureSize.Custom) {
			return(Lighting2D.Profile.qualitySettings.lightTextureSize == LightingSettings.LightingSourceTextureSize.PixelPerfect);
		}

		return (textureSize == LightingSourceTextureSize.PixelPerfect);
	}

	public LightingBuffer2D GetBuffer() {
		if (buffer == null) { 
			buffer = Buffers.PullBuffer (GetTextureSize(), this);
		}
		
		return(buffer);
	}

	public void UpdateLoop() {
		transform2D.Update(this);

		// If Camera Moves
		if (IsPixelPerfect()) {
			transform2D.UpdateNeeded = true;
		}
		
		UpdateBuffer();

		DrawMeshMode();

		if (eventPresetId > 0) {
			EventPreset eventPreset = GetEventPreset();

			if (eventPreset != null) {
				eventHandling.eventHandlingObject.Update(this, eventPreset);
			}
		}
	}

	void BufferUpdate() {
		transform2D.UpdateNeeded = false;

		if (Lighting2D.disable == true) {
			return;
		}

		if (buffer == null) {
			return;
		}
		
		buffer.updateNeeded = true;
	}

	void UpdateCollidersInside() {
		foreach(LightCollider2D collider in collidersInside) {
			if (collider == null) {
				collidersInsideRemove.Add(collider);
				continue;
			}

			if (collider.isActiveAndEnabled == false) {
				collidersInsideRemove.Add(collider);
				continue;
			}

			if (collider.InLight(this) == false) {
				collidersInsideRemove.Add(collider);
			}
		}

		foreach(LightCollider2D collider in collidersInsideRemove) {
			collidersInside.Remove(collider);
			
			transform2D.UpdateNeeded = true;

			if (eventPresetId > 0) {
				if (collider != null) {
					if (collider.lightOnExit != null) {
						collider.lightOnExit.Invoke(this);
					}
				}
			}
		}

		collidersInsideRemove.Clear();
	}

	void UpdateBuffer() {
		UpdateCollidersInside();
		
		if (InAnyCamera()) {
			if (GetBuffer() == null) {
				return;
			}

			if (transform2D.UpdateNeeded == true || inScreen == false) {
				BufferUpdate();

				inScreen = true;
			}
			
		} else {
			if (buffer != null) {
				Buffers.FreeBuffer(buffer);
			}
			
			inScreen = false;
		}
		
	}

	public void DrawMeshMode() {
		if (meshMode.enable == false) {
			return;
		}

		if (buffer == null) {
			return;
		}

		if (isActiveAndEnabled == false) {
			return;
		}

		if (InAnyCamera() == false) {
			return;
		}
		
		LightingMeshRenderer lightingMesh = MeshRendererManager.Pull(this);
		
		if (lightingMesh != null) {
			lightingMesh.UpdateLight(this, meshMode);
		}	
	}

	void OnDrawGizmosSelected() {
		if (Lighting2D.ProjectSettings.editorView.drawGizmos != EditorDrawGizmos.Selected) {
			return;
		}
		
		Draw();
    }

	private void OnDrawGizmos() {
		if (Lighting2D.ProjectSettings.editorView.drawGizmos == EditorDrawGizmos.Disabled) {
			return;
		}
		
		Gizmos.DrawIcon(transform.position, "light_v2", true);

		if (Lighting2D.ProjectSettings.editorView.drawGizmos != EditorDrawGizmos.Always) {
			return;
		}

		Draw();
	}

	void Draw() {
		if (isActiveAndEnabled == false) {
			return;
		}
		
		Gizmos.color = new Color(1f, 0.5f, 0.25f);
	
		if (applyRotation) {
			GizmosHelper.DrawCircle(transform.position, transform2D.rotation, spotAngle, size);
		} else {
			GizmosHelper.DrawCircle(transform.position, 0, spotAngle, size);
		}
		

		Gizmos.color = new Color(0, 1f, 1f);
		
		switch(Lighting2D.ProjectSettings.editorView.drawGizmosBounds) {
			case EditorGizmosBounds.Rectangle:
				GizmosHelper.DrawRect(transform.position, GetWorldRect());
			break;
		}
	}
}