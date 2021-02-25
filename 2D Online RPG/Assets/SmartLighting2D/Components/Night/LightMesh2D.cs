using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LightingSettings;

[ExecuteInEditMode]
public class LightMesh2D : MonoBehaviour {
    public int nightLayer = 0;
    
    public int lightLayer = 0;

    public Color color = Color.white;

    public float size = 5;
	public int segments = 32;

	public bool useUVColor = false;
    public bool useUV = false;
    public Sprite sprite;

    public Material[] materials = new Material[1];

    public MeshMode meshMode = new MeshMode();

    public LightMeshGeometry geometry = new LightMeshGeometry();

    public static List<LightMesh2D> List = new List<LightMesh2D>();

	public LightMeshTransform transform2D = new LightMeshTransform();

	public List<LightCollider2D> collidersInside = new List<LightCollider2D>();
	public List<LightCollider2D> collidersInsideRemove = new List<LightCollider2D>();

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

			if (collider.InLightMesh(this) == false) {
				collidersInsideRemove.Add(collider);
			}
		}

		foreach(LightCollider2D collider in collidersInsideRemove) {
			collidersInside.Remove(collider);
			
			transform2D.UpdateNeeded = true;
		}

		collidersInsideRemove.Clear();
	}

	
	public void AddCollider(LightCollider2D id) {
		if (collidersInside.IndexOf(id) < 0) {
			collidersInside.Add(id);
		}
	}


	private void OnEnable() {
        List.Add(this);

        geometry.Initialize(this);
    }

	private void OnDisable() => List.Remove(this);

    public void UpdateLoop() {
		UpdateCollidersInside();
		
		transform2D.Update(this);

		if (transform2D.UpdateNeeded) {
        	transform2D.UpdateNeeded = false;
		
			geometry.Update();
		}

        DrawMeshMode();
    }

	public void ForceUpdate() {
		transform2D.ForceUpdate();
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

	public Rect GetWorldRect() {
		Rect rect = new Rect();

		rect.x = transform.position.x - size;
		rect.y = transform.position.y - size;
		rect.width = size * 2;
		rect.height = size * 2;

		return(rect);
	}

    public void DrawMeshMode() {
        if (meshMode.enable == false) {
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
			lightingMesh.UpdateLightMesh(this, meshMode);
		}	
    }

    void OnDrawGizmosSelected() {
		if (Lighting2D.ProjectSettings.editorView.drawGizmos != EditorDrawGizmos.Selected) {
			return;
		}

        if (isActiveAndEnabled == false) {
			return;
		}
		
		Gizmos.color = new Color(1f, 0.5f, 0.25f);
		Vector3 center = transform.position;
	
        float angle = 360;

		int start = -(int)(angle / 2);
		int end = (int)(angle / 2);

		for(int i = 0; i < geometry.optimizedPointsCount; i++) {
            Vector2 pointA =  geometry.optimizedPoints[(i) % geometry.optimizedPointsCount];
            Vector2 pointB =  geometry.optimizedPoints[(i + 1) % geometry.optimizedPointsCount];

			Gizmos.DrawLine(pointA, pointB);
		}
    }

	private void OnDrawGizmos() {
		if (Lighting2D.ProjectSettings.editorView.drawGizmos == EditorDrawGizmos.Disabled) {
			return;
		}
		
		Gizmos.DrawIcon(transform.position, "light_v2", true);

		if (Lighting2D.ProjectSettings.editorView.drawGizmos != EditorDrawGizmos.Always) {
			return;
		}

        if (isActiveAndEnabled == false) {
			return;
		}
		
		Gizmos.color = new Color(1f, 0.5f, 0.25f);
		Vector3 center = transform.position;
	
        float angle = 360;

		int start = -(int)(angle / 2);
		int end = (int)(angle / 2);

		for(int i = 0; i < geometry.optimizedPointsCount; i++) {
            Vector2 pointA =  geometry.optimizedPoints[(i) % geometry.optimizedPointsCount];
            Vector2 pointB =  geometry.optimizedPoints[(i + 1) % geometry.optimizedPointsCount];

			Gizmos.DrawLine(pointA, pointB);
		}
	}
}
