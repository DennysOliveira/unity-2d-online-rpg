using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LightTilemapCollider;
using UnityEngine.Tilemaps;

namespace FogOfWar {
	
	public static class NoSort {

		public static void Draw(Camera camera) {
			Material material = null;

			Vector2 objectPosition = new Vector2();

			foreach(FogOfWarSprite sprite in FogOfWarSprite.List) {
				objectPosition.x = sprite.transform.position.x - camera.transform.position.x;
				objectPosition.y = sprite.transform.position.y - camera.transform.position.y;

				SpriteRenderer spriteRenderer = sprite.GetSpriteRenderer();

				if (spriteRenderer == null || sprite.GetSprite() == null) {
					continue;
				}

				material = spriteRenderer.sharedMaterial;
				material.mainTexture = sprite.GetSprite().texture;

				GLExtended.SetColor(spriteRenderer.color);

				Rendering.Universal.Sprite.FullRect.Simple.Draw(sprite.spriteMeshObject, material, sprite.GetSpriteRenderer(), objectPosition, sprite.transform.lossyScale, sprite.transform.rotation.eulerAngles.z);			
			
				GLExtended.ResetColor();
			}

			foreach(FogOfWarTilemap tilemap in FogOfWarTilemap.List) {
				TilemapRenderer tilemapRenderer = tilemap.GetTilemapRenderer();

				material = tilemapRenderer.sharedMaterial;

				switch(tilemap.mapType) {
					case MapType.UnityRectangle:

						Sprite.Draw(camera, tilemap, material);

					break;	

					case MapType.SuperTilemapEditor:
						//SuperTilemapEditorSupport.RenderingRoom.DrawTiles(camera, id, material);

					break;
				}
			}
		}

		public class Sprite {
            
            public static VirtualSpriteRenderer spriteRenderer = new VirtualSpriteRenderer();

            public static void Draw(Camera camera, FogOfWarTilemap id, Material material) {
                Vector2 cameraPosition = -camera.transform.position;

                float cameraRadius = CameraTransform.GetRadius(camera);

                LightTilemapCollider.Base tilemapCollider = id.GetCurrentTilemap();

                material.mainTexture = null; 

                Texture2D currentTexture = null;
        
                GL.Begin (GL.QUADS);

                foreach(LightingTile tile in id.rectangle.mapTiles) {
                    if (tile.GetOriginalSprite() == null) {
                       continue;
                    }

                    Vector2 tilePosition = tile.GetWorldPosition(tilemapCollider);

                    tilePosition += cameraPosition;

                    if (tile.NotInRange(tilePosition, cameraRadius)) {
                       continue;
                    }

                    spriteRenderer.sprite = tile.GetOriginalSprite();

                    if (spriteRenderer.sprite.texture == null) {
                        continue;
                    }
                    
                    if (currentTexture != spriteRenderer.sprite.texture) {
                        currentTexture = spriteRenderer.sprite.texture;
                        material.mainTexture = currentTexture;

                        material.SetPass(0);
                    }
        
                    Rendering.Universal.Sprite.FullRect.Simple.DrawPass(tile.spriteMeshObject, spriteRenderer, tilePosition, tile.worldScale, tile.worldRotation);
                }

                GL.End();

                material.mainTexture = null;
            }
		 }
	}
}