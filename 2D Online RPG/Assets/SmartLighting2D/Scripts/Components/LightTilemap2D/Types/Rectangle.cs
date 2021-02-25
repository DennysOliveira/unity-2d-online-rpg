using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace LightTilemapCollider {
	
	[System.Serializable]
    public class Rectangle : Base {

		public List<Polygon2> compositeColliders = new List<Polygon2>();

		private Tilemap tilemap2D;

		public override MapType TilemapType() {
			return(MapType.UnityRectangle);
		}

        public static ITilemap GetITilemap(Tilemap tilemap) {
			ITilemap iTilemap = (ITilemap) FormatterServices.GetUninitializedObject(typeof(ITilemap));
			typeof(ITilemap).GetField("m_Tilemap", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(iTilemap, tilemap);
			return iTilemap;
		}

		// That is not complete
		override public bool IsPhysicsShape() {
			if (maskType == MaskType.SpritePhysicsShape) {
				return(true);
			}

			if (shadowType == ShadowType.SpritePhysicsShape) {
				return(true);
			}
			return(false);
		}

        public override void Initialize() {
			base.Initialize();
			
			if (UpdateProperties() == false) {
				return;
			}
						
			Tilemap tilemap2D = properties.tilemap;

			TilemapCollider2D tilemapCollider = gameObject.GetComponent<TilemapCollider2D>();
			if (tilemapCollider != null) {
				properties.colliderOffset = tilemapCollider.offset;
			}

			properties.cellAnchor += properties.colliderOffset;

			InitializeGrid();
			InitializeCompositeCollider();

			chunkManager.Update(mapTiles, this);
        }

		private void InitializeCompositeCollider() {
			compositeColliders.Clear();
			
			CompositeCollider2D compositeCollider2D = gameObject.GetComponent<CompositeCollider2D>();

			if (compositeCollider2D != null) {
				compositeColliders = Polygon2Collider2D.CreateFromCompositeCollider(compositeCollider2D);
			}
		}
		
		public override Vector2 TileWorldPosition(LightingTile tile) {
			Vector2 position = tilemap2D.CellToWorld(tile.gridPosition);

			float rotation = properties.cellAnchor.Atan2() + tilemap2D.transform.eulerAngles.z * Mathf.Deg2Rad;
			float sizeX = properties.cellAnchor.x;
			float sizeY = properties.cellAnchor.y;
			
			float distance = Mathf.Sqrt(sizeX * sizeX + sizeY * sizeY);

			// +++ Include Cell Size
			
			position = position.Push(rotation, distance);

			return(position);
		}

		public override float TileWorldRotation(LightingTile tile) {
			float worldRotation = tilemap2D.transform.eulerAngles.z;

			return(worldRotation);
		}

		public override Vector2 TileWorldScale() {
            Transform transform = properties.transform;

            Vector2 scale = Vector2.one;

            scale.x *= transform.lossyScale.x; 
            scale.y *= transform.lossyScale.y;

			bool isGrid = false;
            if (isGrid) {
                scale.x *= properties.cellSize.x;
                scale.y *= properties.cellSize.y;
            }

            return(scale);
        }

		private void InitializeGrid() {
			mapTiles.Clear();

			tilemap2D = properties.tilemap;
			ITilemap tilemap = GetITilemap(tilemap2D);

			foreach (Vector3Int position in tilemap2D.cellBounds.allPositionsWithin) {
				TileData tileData = new TileData();

				TileBase tilebase = tilemap2D.GetTile(position);

				if (tilebase != null) {

					tilebase.GetTileData(position, tilemap, ref tileData);
					
					LightingTile lightingTile = new LightingTile();
				
					lightingTile.gridPosition = position;

					lightingTile.rotation = tilemap2D.GetTransformMatrix(position).rotation.eulerAngles.z;

					lightingTile.SetOriginalSprite(tileData.sprite);
					lightingTile.GetPhysicsShapePolygons();

					lightingTile.colliderType = tileData.colliderType;

					mapTiles.Add(lightingTile);
				}
			}
		}
    }
}