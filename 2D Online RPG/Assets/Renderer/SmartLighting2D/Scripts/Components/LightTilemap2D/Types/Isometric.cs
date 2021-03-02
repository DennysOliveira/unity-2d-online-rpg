using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace LightTilemapCollider {

	[System.Serializable]
    public class Isometric : Base {

		public bool ZasY = false;
		private Tilemap tilemap2D;

		public override MapType TilemapType() {
			return(MapType.UnityIsometric);
		}
	
        public override void Initialize() {
			base.Initialize();
			
			if (UpdateProperties() == false) {
				return;
			}

			mapTiles.Clear();

			tilemap2D = properties.tilemap;

			ITilemap tilemap = (ITilemap) FormatterServices.GetUninitializedObject(typeof(ITilemap));
			typeof(ITilemap).GetField("m_Tilemap", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(tilemap, tilemap2D);

			foreach (Vector3Int position in tilemap2D.cellBounds.allPositionsWithin) {
				TileData tileData = new TileData();

				TileBase tilebase = tilemap2D.GetTile(position);

				if (tilebase != null) {
					tilebase.GetTileData(position, tilemap, ref tileData);

					LightingTile lightingTile = new LightingTile();
					lightingTile.gridPosition = new Vector3Int(position.x, position.y, position.z);

					lightingTile.scale = tilemap.GetTransformMatrix(position).lossyScale;

					lightingTile.SetOriginalSprite(tileData.sprite);
					lightingTile.GetPhysicsShapePolygons();

					mapTiles.Add(lightingTile);
				}
			}

			chunkManager.Update(mapTiles, this);
		}

		public override Vector2 TileWorldPosition(LightingTile tile) {
			Vector2 tilemapOffset = properties.transform.position;
            
            // Tile Offset
            Vector2 tileOffset = new Vector2(tile.gridPosition.x, tile.gridPosition.y);
       
            tileOffset.x += properties.cellAnchor.x;
            tileOffset.y += properties.cellAnchor.y;

            tileOffset.x += properties.cellGap.x * tile.gridPosition.x;
            tileOffset.y += properties.cellGap.y * tile.gridPosition.y;
            
            // Tile Position
            Vector2 tilePosition = tilemapOffset;
            
            tilePosition.x += tileOffset.x * 0.5f;
            tilePosition.x += tileOffset.y * -0.5f;
            tilePosition.x *= properties.cellSize.x;

            tilePosition.y += tileOffset.x * 0.5f * properties.cellSize.y;
            tilePosition.y += tileOffset.y * 0.5f * properties.cellSize.y;

            if (ZasY) {
                tilePosition.y += tile.gridPosition.z * 0.25f;
            }
     
            tilePosition.x *= properties.transform.lossyScale.x;
            tilePosition.y *= properties.transform.lossyScale.y;

			return(tilePosition);
		}

		public override float TileWorldRotation(LightingTile tile) {
			float worldRotation = tilemap2D.transform.eulerAngles.z;

			return(worldRotation);
		}

		public override Vector2 TileWorldScale() {
            Vector2 scale = new Vector2();

            scale.x = properties.transform.lossyScale.x; //properties.cellSize.x * 
            scale.y = properties.transform.lossyScale.y; // properties.cellSize.y *

            return(scale);
        }

		override public bool IsPhysicsShape() {
			if (maskType == MaskType.SpritePhysicsShape) {
				return(true);
			}

			if (shadowType == ShadowType.SpritePhysicsShape) {
				return(true);
			}
			return(false);
		}
    }
}