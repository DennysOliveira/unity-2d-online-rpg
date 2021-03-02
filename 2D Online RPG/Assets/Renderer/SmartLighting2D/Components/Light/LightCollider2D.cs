using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using LightingSettings;
using EventHandling;
using UnityEngine.Events;
using LightSettings;

[ExecuteInEditMode]
public class LightCollider2D : MonoBehaviour {
	public enum ShadowType {None, SpritePhysicsShape, CompositeCollider2D, Collider2D, Collider3D, MeshRenderer, SkinnedMeshRenderer};
	public enum MaskType {None, Sprite, BumpedSprite,  SpritePhysicsShape, CompositeCollider2D, Collider2D, Collider3D, MeshRenderer, BumpedMeshRenderer, SkinnedMeshRenderer};
	public enum MaskPivot {TransformCenter, ShapeCenter, LowestY};

	// Shadow
	public ShadowType shadowType = ShadowType.SpritePhysicsShape;
	public int shadowLayer = 0;

	[Min(0)]
	public float shadowDistance = 0;

	[Range(0, 1)]
	public float shadowTranslucency = 0;

	// Mask
	public MaskType maskType = MaskType.Sprite;
	public MaskEffect maskEffect = MaskEffect.Lit;
	public MaskPivot maskPivot = MaskPivot.TransformCenter;
	public int maskLayer = 0;

	[Range(0, 1)]
	public float maskTranslucency = 1;

	public BumpMapMode bumpMapMode = new BumpMapMode();

	public bool applyToChildren = false;

	public event CollisionEvent2D collisionEvents;
	public bool usingEvents = false;
	
	public LightColliderShape mainShape = new LightColliderShape();
	public List<LightColliderShape> shapes = new List<LightColliderShape>();

	public SpriteMeshObject spriteMeshObject = new SpriteMeshObject();

	public LightEvent lightOnEnter;
	public LightEvent lightOnExit;

	// List Manager 
	public static List<LightCollider2D> List = new List<LightCollider2D>();
	public static List<LightCollider2D> ListEventReceivers = new List<LightCollider2D>();
	public static LayerManager<LightCollider2D> layerManagerMask = new LayerManager<LightCollider2D>();
	public static LayerManager<LightCollider2D> layerManagerShadow = new LayerManager<LightCollider2D>();
	
	private int listMaskLayer = -1;
	private int listShadowLayer = -1;

	public bool ShadowDisabled() {
		return(mainShape.shadowType == LightCollider2D.ShadowType.None);
	}

	public void AddEventOnEnter(UnityAction<Light2D> call) {
		 if (lightOnEnter == null) {
			 lightOnEnter = new LightEvent();
		 }

        lightOnEnter.AddListener(call);
	}

	public void AddEventOnExit(UnityAction<Light2D> call) {
		 if (lightOnExit == null) {
			 lightOnExit = new LightEvent();
		 }

        lightOnExit.AddListener(call);
	}

	public void AddEvent(CollisionEvent2D collisionEvent) {
		collisionEvents += collisionEvent;

		ListEventReceivers.Add(this);

		usingEvents = true;
	}

	public void RemoveEvent(CollisionEvent2D collisionEvent) {
		ListEventReceivers.Remove(this);

		collisionEvent -= collisionEvent;
	}

	public static void ForceUpdateAll() {
		foreach (LightCollider2D lightCollider2D in LightCollider2D.List) {
			lightCollider2D.Initialize();
		}
	}
	
	private void OnEnable() {
		List.Add(this);

		UpdateLayerList();

		LightingManager2D.Get();

		Initialize();

		UpdateNearbyLights();

		bumpMapMode.SetSpriteRenderer(mainShape.spriteShape.GetSpriteRenderer());
	}

	private void OnDisable() {
		List.Remove(this);
		
		ClearLayerList();
		
		UpdateNearbyLights();
	}

	private void OnDestroy() {
		List.Remove(this);
		
		if (ListEventReceivers.Count > 0) {
			if (ListEventReceivers.Contains(this)) {
				ListEventReceivers.Remove(this);
			}
		}
		
	}









	public void Update() {
		UpdateLayerList();
	}


	// Layer List
	void ClearLayerList() {
		layerManagerMask.Remove(listMaskLayer, this);
		layerManagerShadow.Remove(listShadowLayer, this);

		listMaskLayer = -1;
		listShadowLayer = -1;
	}

	void UpdateLayerList() {
		listMaskLayer = layerManagerMask.Update(listMaskLayer, maskLayer, this);
		listShadowLayer = layerManagerShadow.Update(listShadowLayer, shadowLayer, this);
	}

	static public List<LightCollider2D> GetMaskList(int layer) {
		return(layerManagerMask.layerList[layer]);
	}

	static public List<LightCollider2D> GetShadowList(int layer) {
		return(layerManagerShadow.layerList[layer]);
	}
	







	public void CollisionEvent(LightCollision2D collision) {
		if (collisionEvents != null) {
			collisionEvents (collision);
		}
	}

	public bool InLight(Light2D light) {
		Rect lightRect = light.GetWorldRect();

		return(mainShape.RectOverlap(lightRect));
	}

	public bool InLightMesh(LightMesh2D light) {
		Rect lightRect = light.GetWorldRect();

		return(mainShape.RectOverlap(lightRect));
	}

	public void UpdateNearbyLights() {
		foreach (Light2D id in Light2D.List) {
			if (DrawOrNot(id) == false) {
				continue;
			}

			if (InLight(id)) {
				id.ForceUpdate();
			}
		}

		foreach (LightMesh2D id in LightMesh2D.List) {
			if (shadowLayer != id.lightLayer) {
				continue;
			}
	
			if (InLightMesh(id)) {
				id.ForceUpdate();
			}
		}
	}

	 private void AddChildShapes(Transform parent) {
        foreach (Transform child in parent) {
			LightColliderShape shape = new LightColliderShape();
			shape.maskType = mainShape.maskType;
			shape.maskPivot = mainShape.maskPivot;
			shape.maskTranslucency = mainShape.maskTranslucency;

			shape.shadowType = mainShape.shadowType;
			shape.shadowDistance = mainShape.shadowDistance;
			shape.shadowTranslucency = mainShape.shadowTranslucency;
			
			shape.SetTransform(child);
			shape.transform2D.Update();
			
			shapes.Add(shape);

			AddChildShapes(child);
        }
    }


	public void Initialize() {
		shapes.Clear();

		mainShape.maskType = maskType;
		mainShape.maskPivot = maskPivot;
		mainShape.maskTranslucency = maskTranslucency;

		mainShape.shadowType = shadowType;
		mainShape.shadowDistance = shadowDistance;
		mainShape.shadowTranslucency = shadowTranslucency;

		mainShape.SetTransform(transform);
		mainShape.transform2D.Reset();
		mainShape.transform2D.Update();
		mainShape.transform2D.UpdateNeeded = true;

		shapes.Add(mainShape);

		if (applyToChildren) {
			AddChildShapes(transform);
		}

		foreach(LightColliderShape shape in shapes) {
			shape.ResetLocal();
		}
	}

	public bool DrawOrNot(Light2D id) {
		LayerSetting[] layerSetting = id.GetLayerSettings();

		if (layerSetting == null) {
			return(false);
		}

		for(int i = 0; i < layerSetting.Length; i++) {
			if (layerSetting[i] == null) {
				continue;
			}

			int layerID = layerSetting[i].layerID;
			
			switch(layerSetting[i].type) {
				case LightLayerType.ShadowAndMask:
					if (layerID == shadowLayer || layerID == maskLayer) {
						return(true);
					}
				break;

				case LightLayerType.MaskOnly:
					if (layerID == maskLayer) {
						return(true);
					}
				break;

				case LightLayerType.ShadowOnly:
					if (layerID  == shadowLayer) {
						return(true);
					}
				break;
			}
		}

		return(false);
	}

	public void UpdateLoop() {
		bool updateLights = false;

		foreach(LightColliderShape shape in shapes) {
			shape.transform2D.Update();

			if (shape.transform2D.UpdateNeeded) {
				shape.transform2D.UpdateNeeded = false;
				
				shape.ResetWorld();

				updateLights = true;
			}
		}

		if (updateLights) {
			UpdateNearbyLights();
		}

	}

	void OnDrawGizmosSelected() {
		if (Lighting2D.ProjectSettings.editorView.drawGizmos != EditorDrawGizmos.Selected) {
			return;
		}

		DrawGizmos();
    }

	private void OnDrawGizmos() {
		if (Lighting2D.ProjectSettings.editorView.drawGizmos != EditorDrawGizmos.Always) {
			return;
		}

		DrawGizmos();
	}
	
	private void DrawGizmos() {
		if (isActiveAndEnabled == false) {
			return;
		}

		Gizmos.color = new Color(1f, 0.5f, 0.25f);
		
		if (mainShape.shadowType != LightCollider2D.ShadowType.None) {
			foreach(LightColliderShape shape in shapes) {
				List<Polygon2> polygons = shape.GetPolygonsWorld();
				
				GizmosHelper.DrawPolygons(polygons, transform.position);
			}
		}

		switch(Lighting2D.ProjectSettings.editorView.drawGizmosBounds) {
			case EditorGizmosBounds.Rectangle:
			
				if (maskEffect == MaskEffect.Isometric) {
					Gizmos.color = Color.green;
					GizmosHelper.DrawIsoRect(transform.position, mainShape.GetIsoWorldRect());
				} else {
					Gizmos.color = new Color(0, 1f, 1f);
					GizmosHelper.DrawRect(transform.position, mainShape.GetWorldRect());
				}
				
			break;
		}

		Vector2? pivotPoint = mainShape.GetPivotPoint();

		if (pivotPoint != null) {
			Vector3 pos = transform.position;
			pos.x = pivotPoint.Value.x;
			pos.y = pivotPoint.Value.y;

			Gizmos.DrawIcon(pos, "circle_v2", true);
		}
	}
}