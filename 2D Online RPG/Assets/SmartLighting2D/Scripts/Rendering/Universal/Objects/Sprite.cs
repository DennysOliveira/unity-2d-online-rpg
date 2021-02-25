using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rendering.Universal {

	public class Sprite : Base {
        static VirtualSpriteRenderer virtualSpriteRenderer = new VirtualSpriteRenderer();

		static  public void Draw(SpriteMeshObject spriteMeshObject, Material material, SpriteRenderer spriteRenderer, Vector2 position, Vector2 scale, float rotation) {
			if (spriteRenderer == null) {
				return;
			}

			if (spriteRenderer.sprite == null) {
				return;
			}
			
			//if (spriteRenderer.sprite.packingMode == SpritePackingMode.Tight) {
				// FullRect.Draw(spriteMeshObject, material, spriteRenderer, position, scale,  rotation);
			//} else {
				FullRect.Draw(spriteMeshObject, material, spriteRenderer, position, scale,  rotation);
			//}
		}

		public class Tight {
			// ??
		}

		public class FullRect {

			public class Simple {

				static public void DrawPass(SpriteMeshObject spriteMeshObject, SpriteRenderer spriteRenderer, Vector2 position, Vector2 scale, float rotation) {
					virtualSpriteRenderer.Set(spriteRenderer);
					
					DrawPass(spriteMeshObject,virtualSpriteRenderer, position, scale, rotation);
				}

				static public void DrawPass(SpriteMeshObject spriteMeshObject, VirtualSpriteRenderer spriteRenderer, Vector2 position, Vector2 scale, float rotation) {
					SpriteTransform spriteTransform = new SpriteTransform(spriteRenderer, position, scale, rotation);

					Texture.DrawPass(spriteTransform.position, spriteTransform.scale, spriteTransform.uv, rotation, 0);
				}
				
				static public void Draw(SpriteMeshObject spriteMeshObject, Material material, SpriteRenderer spriteRenderer, Vector2 position, Vector2 scale, float rotation) {
					virtualSpriteRenderer.Set(spriteRenderer);
					
					Draw(spriteMeshObject, material, virtualSpriteRenderer, position, scale, rotation);
				}

				static public void Draw(SpriteMeshObject spriteMeshObject, Material material, VirtualSpriteRenderer spriteRenderer, Vector2 position, Vector2 scale, float rotation) {
					SpriteTransform spriteTransform = new SpriteTransform(spriteRenderer, position, scale, rotation);

					Texture.Draw(material, spriteTransform.position, spriteTransform.scale, spriteTransform.uv, rotation, 0);
				}
			}

			public class Tiled {
				static public void Draw(SpriteMeshObject spriteMeshObject, Material material, SpriteRenderer spriteRenderer, Vector2 pos, Vector2 size, float rotation) {
					material.SetPass (0); 
					GLExtended.DrawMesh(spriteMeshObject.GetTiledMesh().GetMesh(spriteRenderer), pos, size, rotation);
				}

				static public void DrawPass(SpriteMeshObject spriteMeshObject, SpriteRenderer spriteRenderer, Vector2 pos, Vector2 size, float rotation) {
					GLExtended.DrawMesh(spriteMeshObject.GetTiledMesh().GetMesh(spriteRenderer), pos, size, rotation);
				}
			}

			static public void Draw(SpriteMeshObject spriteMeshObject, Material material, SpriteRenderer spriteRenderer, Vector2 pos, Vector2 size, float rotation) {
				if (spriteRenderer.drawMode == SpriteDrawMode.Tiled && spriteRenderer.tileMode == SpriteTileMode.Continuous) {
					Tiled.Draw(spriteMeshObject, material, spriteRenderer, pos, size, rotation);
				} else {
					Simple.Draw(spriteMeshObject, material, spriteRenderer, pos, size, rotation);
				}
			}

			static public void DrawPass(SpriteMeshObject spriteMeshObject, SpriteRenderer spriteRenderer, Vector2 pos, Vector2 size, float rotation) {
				if (spriteRenderer.drawMode == SpriteDrawMode.Tiled && spriteRenderer.tileMode == SpriteTileMode.Continuous) {
					Tiled.DrawPass(spriteMeshObject, spriteRenderer, pos, size, rotation);
				} else {
					Simple.DrawPass(spriteMeshObject, spriteRenderer, pos, size, rotation);
				}
			}
		}	
    }
}