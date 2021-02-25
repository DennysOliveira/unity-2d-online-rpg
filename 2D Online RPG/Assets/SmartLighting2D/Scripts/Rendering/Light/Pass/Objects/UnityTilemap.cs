using UnityEngine;
using LightSettings;

namespace Rendering.Light {

    public class UnityTilemap {
        public static VirtualSpriteRenderer virtualSpriteRenderer = new VirtualSpriteRenderer();
        
        static public void Sprite(Light2D light, LightTilemapCollider2D id, Material material, LayerSetting layerSetting) {
            Vector2 lightPosition = -light.transform.position;

            LightTilemapCollider.Base tilemap = id.GetCurrentTilemap();
            Vector2 scale = tilemap.TileWorldScale();
            float rotation = id.transform.eulerAngles.z;
            
            int count = tilemap.chunkManager.GetTiles(light.GetWorldRect());

            Texture2D currentTexture = null;
            Color currentColor = Color.black;

            GL.Begin(GL.QUADS);

            for(int i = 0; i < count; i++) {
                LightingTile tile = tilemap.chunkManager.display[i];

                if (tile.GetOriginalSprite() == null) {
                    return;
                }

                Vector2 tilePosition = tile.GetWorldPosition(tilemap);

                tilePosition += lightPosition;

                if (tile.NotInRange(tilePosition, light.size)) {
                    continue;
                }

                virtualSpriteRenderer.sprite = tile.GetOriginalSprite();

                if (virtualSpriteRenderer.sprite.texture == null) {
                    continue;
                }
                
                Color color = LayerSettingColor.Get(tilePosition, layerSetting, MaskEffect.Lit, 1); // 1?

                if (currentTexture != virtualSpriteRenderer.sprite.texture || currentColor != color) {
                    currentTexture = virtualSpriteRenderer.sprite.texture;
                    currentColor = color;

                    material.mainTexture = currentTexture;
                    material.color = currentColor;

                    material.SetPass(0);
                }
    
                Universal.Sprite.FullRect.Simple.Draw(tile.spriteMeshObject, material, virtualSpriteRenderer, tilePosition, scale * tile.scale, rotation + tile.rotation);
            }

             GL.End();

            material.mainTexture = null;
        }

        static public void BumpedSprite(Light2D light, LightTilemapCollider2D id, Material material, LayerSetting layerSetting) {
            Texture bumpTexture = id.bumpMapMode.GetBumpTexture();

            if (bumpTexture == null) {
                return;
            }

            material.SetTexture("_Bump", bumpTexture);

            Vector2 lightPosition = -light.transform.position;

            LightTilemapCollider.Base tilemap = id.GetCurrentTilemap();
            Vector2 scale = tilemap.TileWorldScale();

            Texture2D currentTexture = null;

            GL.Begin(GL.QUADS);

            foreach(LightingTile tile in tilemap.mapTiles) {
                if (tile.GetOriginalSprite() == null) {
                    return;
                }

                Vector2 tilePosition = tilemap.TileWorldPosition(tile);

                tilePosition += lightPosition;

                if (tile.NotInRange(tilePosition, light.size)) {
                    continue;
                }

                virtualSpriteRenderer.sprite = tile.GetOriginalSprite();

                if (virtualSpriteRenderer.sprite.texture == null) {
                    continue;
                }

                if (currentTexture != virtualSpriteRenderer.sprite.texture) {
                    currentTexture = virtualSpriteRenderer.sprite.texture;
                    material.mainTexture = currentTexture;

                    material.SetPass(0);
                }

                Color color = LayerSettingColor.Get(tilePosition, layerSetting, MaskEffect.Lit, 1); // 1

                GLExtended.SetColor(color);
    
                Universal.Sprite.FullRect.Simple.DrawPass(tile.spriteMeshObject, virtualSpriteRenderer, tilePosition, scale, tile.worldRotation);
            }

            GL.End();

            material.mainTexture = null;
        }

        static public void MaskShape(Light2D light, LightTilemapCollider2D id, LayerSetting layerSetting) {
            Vector2 lightPosition = -light.transform.position;
         
            LightTilemapCollider.Base tilemap = id.GetCurrentTilemap();

            bool isGrid = !tilemap.IsPhysicsShape();

            Vector2 scale = tilemap.TileWorldScale();

            float rotation = id.transform.eulerAngles.z;

            MeshObject tileMesh = null;	
            
            if (isGrid) {
                tileMesh = LightingTile.GetStaticMesh(tilemap);
            }

            int count = tilemap.chunkManager.GetTiles(light.GetWorldRect());

            for(int i = 0; i < count; i++) {
                LightingTile tile = tilemap.chunkManager.display[i];

                Vector2 tilePosition = tilemap.TileWorldPosition(tile);

                tilePosition += lightPosition;
                
                if (tile.NotInRange(tilePosition, light.size)) {
                    continue;
                }

                if (isGrid == false) {
                    tileMesh = null;
                    tileMesh = tile.GetDynamicMesh();
                }

                if (tileMesh == null) {
                    continue;
                }
                        
                Color color = LayerSettingColor.Get(tilePosition, layerSetting, MaskEffect.Lit, 1); // 1?

                GL.Color(color);

                GLExtended.DrawMeshPass(tileMesh, tilePosition, scale, rotation + tile.rotation);		
            }

            GL.Color(Color.white);

        }
    }
}