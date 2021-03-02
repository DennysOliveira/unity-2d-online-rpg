using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LightSettings;

namespace Rendering.Light {

    public class Tile {
		public static VirtualSpriteRenderer virtualSpriteRenderer = new VirtualSpriteRenderer();

       	static public void MaskSprite(LightingTile tile, LayerSetting layerSetting, Material material, LightTilemapCollider2D tilemap, float lightSizeSquared) {
			virtualSpriteRenderer.sprite = tile.GetOriginalSprite();

			if (virtualSpriteRenderer.sprite == null) {
				return;
			}

			LightTilemapCollider.Base tilemapBase = tilemap.GetCurrentTilemap();

			Vector2 tilePosition = tile.GetWorldPosition(tilemapBase) - ShadowEngine.light.transform2D.position;

			material.color = LayerSettingColor.Get(tilePosition, layerSetting, MaskEffect.Lit, 1); // 1?

			material.mainTexture = virtualSpriteRenderer.sprite.texture;

			Vector2 scale = tile.worldScale * tile.scale;

			GLExtended.SetColor(Color.white);

			tilePosition += ShadowEngine.drawOffset;

			Universal.Sprite.FullRect.Simple.Draw(tile.spriteMeshObject, material, virtualSpriteRenderer, tilePosition, scale, tile.worldRotation);
			
			material.mainTexture = null;
		}
    }
}