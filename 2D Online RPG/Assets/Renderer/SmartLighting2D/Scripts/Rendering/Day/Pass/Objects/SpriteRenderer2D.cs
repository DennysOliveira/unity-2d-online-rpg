using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rendering.Day {

    public class SpriteRenderer2D {

		static public void Draw(DayLightCollider2D id, Vector2 offset) {
			if (id.InAnyCamera() == false) {
				return;
			}

			float dayLightRotation = -(Lighting2D.DayLightingSettings.direction - 180) * Mathf.Deg2Rad;
			float dayLightHeight = Lighting2D.DayLightingSettings.bumpMap.height;
			float dayLightStrength = Lighting2D.DayLightingSettings.bumpMap.strength;

			switch(id.mainShape.maskType) {
				case DayLightCollider2D.MaskType.None:
					return;

				case DayLightCollider2D.MaskType.Sprite:
				
					Material material = Lighting2D.materials.GetMask();

					foreach(DayLightColliderShape shape in id.shapes) {
						UnityEngine.SpriteRenderer spriteRenderer = shape.spriteShape.GetSpriteRenderer();

						if (spriteRenderer == null || spriteRenderer.sprite == null) {
							continue;
						}

						Vector2 objectOffset = shape.transform2D.position + offset;

						material.mainTexture = spriteRenderer.sprite.texture;

						Universal.Sprite.Draw(id.spriteMeshObject, material, spriteRenderer, objectOffset, shape.transform2D.scale, shape.transform2D.rotation);
					
					}
				break;

				case DayLightCollider2D.MaskType.BumpedSprite:

					Texture bumpTexture = id.normalMapMode.GetBumpTexture();

					if (bumpTexture == null) {
						return;
					}

					material = Lighting2D.materials.GetBumpedDaySprite();
					material.SetFloat("_LightRZ", -dayLightHeight);
					material.SetTexture("_Bump", bumpTexture);

					foreach(DayLightColliderShape shape in id.shapes) {
						UnityEngine.SpriteRenderer spriteRenderer = shape.spriteShape.GetSpriteRenderer();

						if (spriteRenderer == null || spriteRenderer.sprite == null) {
							continue;
						}

						float rotation = dayLightRotation - shape.transform2D.rotation * Mathf.Deg2Rad;
						material.SetFloat("_LightRX", Mathf.Cos(rotation) * dayLightStrength);
						material.SetFloat("_LightRY", Mathf.Sin(rotation) * dayLightStrength);
							
						Vector2 objectOffset = shape.transform2D.position + offset;

						material.mainTexture = spriteRenderer.sprite.texture;

						Universal.Sprite.FullRect.Draw(id.spriteMeshObject, material, spriteRenderer, objectOffset, id.transform.lossyScale, shape.transform2D.rotation);
					}

				break;
			}
		}


		static VirtualSpriteRenderer virtualSpriteRenderer = new VirtualSpriteRenderer();
    
		static public void DrawTilemap(DayLightTilemapCollider2D id, Vector2 offset) {
			//if (id.InAnyCamera() == false) {
			//	return;
			//}

			if (id.rectangle.maskType != LightTilemapCollider.MaskType.Sprite) {
				return;
			}

			LightTilemapCollider.Base tilemap = id.GetCurrentTilemap();

			Vector2 scale = tilemap.TileWorldScale();
            float rotation = id.transform.eulerAngles.z;

			Material material = Lighting2D.materials.GetMask();

            foreach(LightingTile tile in id.rectangle.mapTiles) {

                if (tile.GetOriginalSprite() == null) {
                    return;
                }

				tile.UpdateTransform(tilemap);

                Vector2 tilePosition = tile.GetWorldPosition(tilemap);

                tilePosition += offset;

               // if (tile.NotInRange(tilePosition, light.size)) {
                 //   continue;
                //}

                virtualSpriteRenderer.sprite = tile.GetOriginalSprite();

                material.color = Color.white;
                
                material.mainTexture = virtualSpriteRenderer.sprite.texture;
    
                Universal.Sprite.FullRect.Simple.Draw(tile.spriteMeshObject, material, virtualSpriteRenderer, tilePosition, scale, rotation);
                
                material.mainTexture = null;
            
            }
		}
	}
}