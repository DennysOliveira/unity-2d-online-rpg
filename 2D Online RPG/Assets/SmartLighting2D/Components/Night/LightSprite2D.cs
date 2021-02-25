using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LightingSettings;

[ExecuteInEditMode]
public class LightSprite2D : MonoBehaviour {
	public enum Type {Light, Mask};
	public enum SpriteMode {Custom, SpriteRenderer};

	public int nightLayer = 0;

	public Type type = Type.Light;
	public SpriteMode spriteMode = SpriteMode.Custom;
	public Sprite sprite = null;

    public Color color = new Color(0.5f, 0.5f, 0.5f, 1f);
	
    public bool flipX = false;
    public bool flipY = false;

	public LightSpriteTransform lightSpriteTransform = new LightSpriteTransform();

	public LightSpriteShape lightSpriteShape = new LightSpriteShape();

	public MeshMode meshMode = new MeshMode();

	public GlowMode glowMode = new GlowMode();

	public VirtualSpriteRenderer spriteRenderer = new VirtualSpriteRenderer();

	public SpriteMeshObject spriteMeshObject = new SpriteMeshObject();

	public static List<LightSprite2D> List = new List<LightSprite2D>();

	SpriteRenderer spriteRendererComponent;

	public void OnEnable() {
		List.Add(this);

		LightingManager2D.Get();

		lightSpriteShape.Set(spriteRenderer, transform, lightSpriteTransform);
	}

	public void OnDisable() {
		List.Remove(this);
	}

	public bool InCamera(Camera camera) {
		Rect cameraRect = CameraTransform.GetWorldRect(camera);

		if (cameraRect.Overlaps(lightSpriteShape.GetWorldRect())) {
			return(true);
		}

		return(false);
	}

	public Sprite GetSprite() {
		if (GetSpriteOrigin() == null) {
			return(null);
		}

		if (glowMode.enable) {
			Sprite blurredSprite = GlowManager.RequestSprite(GetSpriteOrigin(), glowMode.glowSize, glowMode.glowIterations);
			if (blurredSprite == null) {
				return(GetSpriteOrigin());
			} else {
				return(blurredSprite);
			}
		} else {
			return(GetSpriteOrigin());
		}
	}

	public Sprite GetSpriteOrigin() {
		if (spriteMode == SpriteMode.Custom) {
			return(sprite);
		} else {
			if (GetSpriteRenderer() == null) {
				return(null);
			}
			sprite = spriteRendererComponent.sprite;

			return(sprite);
		}
	}

	public SpriteRenderer GetSpriteRenderer() {
		if (spriteRendererComponent == null) {
			spriteRendererComponent = GetComponent<SpriteRenderer>();
		}
		return(spriteRendererComponent);
	}

	public void UpdateLoop() {
		if (spriteMode == SpriteMode.SpriteRenderer) {
			SpriteRenderer sr = GetSpriteRenderer();
			if (sr != null) {
				spriteRenderer.flipX = sr.flipX;
				spriteRenderer.flipY = sr.flipY;		
			}
		} else {
			spriteRenderer.flipX = flipX;
			spriteRenderer.flipY = flipY;	
		}

		spriteRenderer.sprite = GetSprite();
		spriteRenderer.color = color;

		if (meshMode.enable) {
			DrawMesh();
		}

		lightSpriteShape.Update();
	}

	public void DrawMesh() {
		if (meshMode.enable == false) {
			return;
		}

		LightingMeshRenderer lightingMesh = MeshRendererManager.Pull(this);

		if (lightingMesh != null) {
			lightingMesh.UpdateLightSprite(this, meshMode);
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
		
		// Gizmos.DrawIcon(transform.position, "light", true);

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

		GizmosHelper.DrawPolygon(lightSpriteShape.GetSpriteWorldPolygon(), transform.position);

		Gizmos.color = new Color(0, 1f, 1f);

		switch(Lighting2D.ProjectSettings.editorView.drawGizmosBounds) {
			case EditorGizmosBounds.Rectangle:	
				GizmosHelper.DrawRect(transform.position, lightSpriteShape.GetWorldRect());  
			break;
		}
	}
}
