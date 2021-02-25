using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using UnityEngine;
using LightingSettings;
using UnityEngine.Tilemaps;
using System;
using LightTilemapCollider;

#if UNITY_2017_4_OR_NEWER

	public enum ShadowTileType {AllTiles, ColliderOnly};

	[ExecuteInEditMode]
	public class LightTilemapCollider2D : MonoBehaviour {
		public MapType mapType = MapType.UnityRectangle;

		public int shadowLayer = 0;
		public int maskLayer = 0;

		public ShadowTileType shadowTileType = ShadowTileType.AllTiles;

		public BumpMapMode bumpMapMode = new BumpMapMode();

		public Rectangle rectangle = new Rectangle();
		public Isometric isometric = new Isometric();
		public Hexagon hexagon = new Hexagon();

		public SuperTilemapEditorSupport.TilemapCollider2D superTilemapEditor = new SuperTilemapEditorSupport.TilemapCollider2D();

		public LightingTilemapTransform lightingTransform = new LightingTilemapTransform();

		public static List<LightTilemapCollider2D> list = new List<LightTilemapCollider2D>();
		public static LayerManager<LightTilemapCollider2D> layerManagerMask = new LayerManager<LightTilemapCollider2D>();
		public static LayerManager<LightTilemapCollider2D> layerManagerCollision = new LayerManager<LightTilemapCollider2D>();

		private int listMaskLayer = -1;
		private int listCollisionLayer = -1;

		static public List<LightTilemapCollider2D> GetMaskList(int layer) {
			return(layerManagerMask.layerList[layer]);
		}

		static public List<LightTilemapCollider2D> GetCollisionList(int layer) {
			return(layerManagerCollision.layerList[layer]);
		}

		// Layer List
		void ClearLayerList() {
			layerManagerMask.Remove(listMaskLayer, this);
			layerManagerCollision.Remove(listCollisionLayer, this);
		
			listMaskLayer = -1;
			listCollisionLayer = -1;
		}

		void UpdateLayerList() {
			listMaskLayer = layerManagerMask.Update(listMaskLayer, maskLayer, this);
			listCollisionLayer = layerManagerCollision.Update(listCollisionLayer, shadowLayer, this);
		}

		public bool ShadowsDisabled() {
			return(GetCurrentTilemap().ShadowsDisabled());
		}

		public bool MasksDisabled() {
			return(GetCurrentTilemap().MasksDisabled());
		}

		public void OnEnable() {
			list.Add(this);

			UpdateLayerList();

			LightingManager2D.Get();

			rectangle.SetGameObject(gameObject);
			isometric.SetGameObject(gameObject);
			hexagon.SetGameObject(gameObject);

			superTilemapEditor.eventsInit = false;
			superTilemapEditor.SetGameObject(gameObject);

			Initialize();

			Light2D.ForceUpdateAll();
		}

		public void OnDisable() {
			list.Remove(this);

			ClearLayerList();

			Light2D.ForceUpdateAll();
		}

		static public List<LightTilemapCollider2D> GetList() {
			return(list);
		}

		public void Update() {
			UpdateLayerList();

			lightingTransform.Update(this);

			if (lightingTransform.UpdateNeeded) {
				GetCurrentTilemap().ResetWorld();

				// Update if light is in range
				foreach(Light2D light in Light2D.List) {
					//if (IsInRange(light)) {
						light.ForceUpdate();
					//}
				}
			}
		}

		/*
		public bool IsInRange(Light2D light) {
			float radius = GetCurrentTilemap().GetRadius() + light.size;
			float distance = Vector2.Distance(light.transform.position, transform.position);

			return(distance < radius);
		}*/

		//public bool IsNotInRange(Light2D light) {
			//float radius = GetCurrentTilemap().GetRadius() + light.size;
			//float distance = Vector2.Distance(light.transform.position, transform.position);

			//return(distance > radius);

		//	return(false);
		//}

		public LightTilemapCollider.Base GetCurrentTilemap() {
			switch(mapType) {
				case MapType.SuperTilemapEditor:
					return(superTilemapEditor);
				case MapType.UnityRectangle:
					return(rectangle);
				case MapType.UnityIsometric:
					return(isometric);
				case MapType.UnityHexagon:
					return(hexagon);
			}
			return(null);
		}

		public void Initialize() {
			rectangle.SetGameObject(gameObject);
			isometric.SetGameObject(gameObject);
			hexagon.SetGameObject(gameObject);

			TilemapEvents.Initialize();

			GetCurrentTilemap().Initialize();
		}

		public List<LightingTile> GetTileList() {
			return(GetCurrentTilemap().mapTiles);
		}

		public TilemapProperties GetTilemapProperties() {
			return(GetCurrentTilemap().Properties);
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

			LightTilemapCollider.Base tilemap = GetCurrentTilemap();

			foreach(LightingTile tile in GetTileList()) {
				GizmosHelper.DrawPolygons(tile.GetWorldPolygons(tilemap), transform.position);
			}

			// GizmosHelper.DrawPolygons(superTilemapEditor.GetWorldColliders(), transform.position);

			Gizmos.color = new Color(0, 1f, 1f);
		
			switch(Lighting2D.ProjectSettings.editorView.drawGizmosBounds) {
				case EditorGizmosBounds.Rectangle:
					GizmosHelper.DrawRect(transform.position, GetCurrentTilemap().GetRect());
				break;
			}
		}
	}

#endif