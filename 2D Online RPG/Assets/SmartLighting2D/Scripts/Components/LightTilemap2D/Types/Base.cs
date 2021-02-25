using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace LightTilemapCollider {

	public enum MapType {UnityRectangle, UnityIsometric, UnityHexagon, SuperTilemapEditor};

	public enum ShadowType {None, Grid, SpritePhysicsShape, CompositeCollider};
	public enum MaskType {None, Grid, Sprite, BumpedSprite, SpritePhysicsShape};

    public class Base {
		public ShadowType shadowType = ShadowType.Grid;
		public MaskType maskType = MaskType.Sprite;

		private float radius = -1;
		private Rect rect = new Rect();
	
        public GameObject gameObject;
		public Transform transform;
        protected TilemapProperties properties = new TilemapProperties();

		public List<LightingTile> mapTiles = new List<LightingTile>();

		public Chunks.TilemapManager chunkManager = new Chunks.TilemapManager();

		// Mask and Shadow Properties
		public bool ShadowsDisabled() {
			return(shadowType == ShadowType.None);
		}

		public virtual bool MasksDisabled() {
			return(maskType == MaskType.None);
		}

		// Tile World Properties
		public virtual Vector2 TileWorldPosition(LightingTile tile) {
			return(Vector2.zero);
		}

		public virtual float TileWorldRotation(LightingTile tile) {
			return(0);
		}

		public virtual Vector2 TileWorldScale() {
            return(Vector2.one);
        }

		public virtual MapType TilemapType() {
			return(MapType.UnityRectangle);
		}
	
        public TilemapProperties Properties {
            get => properties;
        }

		virtual public bool IsPhysicsShape() {
			return(false);
		}

		public virtual void Initialize() {
			radius = -1;
		}

		public void SetGameObject(GameObject gameObject) {
            this.gameObject = gameObject;
			this.transform = gameObject.transform;

			properties.transform = gameObject.transform;
        }

        public bool UpdateProperties() {
			properties.tilemap = gameObject.GetComponent<Tilemap>();

			if (properties.tilemap == null) {
				return(false);
			}

			properties.grid = properties.tilemap.layoutGrid;

			if (properties.grid == null) {
				Debug.LogError("Lighting 2D Error: Lighting Tilemap Collider is missing Grid", gameObject);
				return(false);
			} else {
				properties.cellSize = properties.grid.cellSize;
				properties.cellGap = properties.grid.cellGap;
			}

			properties.cellAnchor = properties.tilemap.tileAnchor;

			return(true);
		}

		public void ResetWorld() {
			rect = new Rect();
			
			foreach(LightingTile tile in mapTiles) {
				tile.ResetWorld();
			}
		}

		public Rect GetRect() {
			if (rect.width < 0.1f) {
				float minX = 100000;
				float minY = 100000;
				float maxX = -100000;
				float maxY = -100000;

				foreach(LightingTile tile in mapTiles) {
					Vector2 id = tile.GetWorldPosition(this);

					minX = Mathf.Min(minX, (float)id.x);
					minY = Mathf.Min(minY, (float)id.y);
					maxX = Mathf.Max(maxX, (float)id.x);
					maxY = Mathf.Max(maxY, (float)id.y);					
				}

				rect.x = minX;
				rect.y = minY;
				rect.width = maxX - minX;
				rect.height = maxY - minY;
			}

			return(rect);
		}
		
		public float GetRadius() {
			if (radius < 0) {
				foreach(LightingTile tile in mapTiles) {
					Vector2 id = tile.GetWorldPosition(this);
				
					radius = Mathf.Max(radius, Vector2.Distance(id, gameObject.transform.position));
				}

			}
			
			return(radius);
		}
    }
}