using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LightTilemapCollider;

 #if (SUPER_TILEMAP_EDITOR)

    namespace SuperTilemapEditorSupport {
        
        public class RenderingColliderMask { 

            public class WithoutAtlas {   
            
                static public void Sprite(Light2D light, LightTilemapCollider2D id, Material material) {
                    Vector2 lightPosition = -light.transform.position;
                    LightTilemapCollider.Base tilemapCollider = id.GetCurrentTilemap();

                    if (id.superTilemapEditor.tilemap != null) {
                        if (id.superTilemapEditor.tilemap.Tileset != null) {
                            material.mainTexture = id.superTilemapEditor.tilemap.Tileset.AtlasTexture;
                        }
                    }
                
                    material.SetPass (0); 
                    GL.Begin (GL.QUADS);
        
                    int count = tilemapCollider.chunkManager.GetTiles(light.GetWorldRect());

                    for(int i = 0; i < count; i++) {
                        LightingTile tile = tilemapCollider.chunkManager.display[i];

                        tile.UpdateTransform(tilemapCollider);
                        
                        Vector2 tilePosition = tile.GetWorldPosition(tilemapCollider);
                        tilePosition += lightPosition;

                        if (tile.NotInRange(tilePosition, light.size)) {
                            continue;
                        }

                        Vector2 scale = tile.worldScale * 0.5f * tile.scale;
                    
                        Rendering.Universal.Texture.DrawPassSTE(tilePosition, scale, tile.uv, tile.worldRotation, 0);
                    }

                    GL.End ();

                    material.mainTexture = null;
                }

                static public void BumpedSprite(Light2D light, LightTilemapCollider2D id, Material material) {
                    Texture bumpTexture = id.bumpMapMode.GetBumpTexture();

                    if (bumpTexture == null) {
                        return;
                    }

                    material.SetTexture("_Bump", bumpTexture);
                    
                    Vector2 lightPosition = -light.transform.position;
                    LightTilemapCollider.Base tilemapCollider = id.GetCurrentTilemap();

                    if (id.superTilemapEditor.tilemap != null) {
                        if (id.superTilemapEditor.tilemap.Tileset != null) {
                            material.mainTexture = id.superTilemapEditor.tilemap.Tileset.AtlasTexture;
                        }
                    }
                
                    material.SetPass (0); 
                    GL.Begin (GL.QUADS);
        
                    foreach(LightingTile tile in id.superTilemapEditor.mapTiles) {
                        tile.UpdateTransform(tilemapCollider);
                        
                        Vector2 tilePosition = tile.GetWorldPosition(tilemapCollider);
                        tilePosition += lightPosition;

                        if (tile.NotInRange(tilePosition, light.size)) {
                            continue;
                        }

                        Vector2 scale = tile.worldScale * 0.5f * tile.scale;

                        Rendering.Universal.Texture.DrawPassSTE(tilePosition, scale, tile.uv, tile.worldRotation, 0);
                    }

                    GL.End ();

                    material.mainTexture = null;
                }

            }
            
            static public void Grid(Light2D light, LightTilemapCollider2D id) {
                if (id.superTilemapEditor.maskTypeSTE != SuperTilemapEditorSupport.TilemapCollider2D.MaskType.Grid) {
                    return;
                }

                Vector2 lightPosition = -light.transform.position;
                MeshObject tileMesh = LightingTile.Rectangle.GetStaticMesh();
          
                GL.Color(Color.white);

                LightTilemapCollider.Base tilemapBase = id.GetCurrentTilemap();

                foreach(LightingTile tile in id.superTilemapEditor.mapTiles) {
                    Vector2 tilePosition = tile.GetWorldPosition(tilemapBase);
                    tilePosition += lightPosition;
                    
                    if (tile.NotInRange(tilePosition, light.size)) {
                        continue;
                    }

                    GLExtended.DrawMeshPass(tileMesh, tilePosition, tile.worldScale, tile.worldRotation);		
                }
            }

        }
    }

#else  

    namespace SuperTilemapEditorSupport {
        public class RenderingColliderMask { 
            static public void Grid(Light2D light, LightTilemapCollider2D id) {}

            public class WithoutAtlas {
                static public void Sprite(Light2D light, LightTilemapCollider2D id, Material material) {}
                static public void BumpedSprite(Light2D light, LightTilemapCollider2D id, Material material) {}
            }
    
         }
    }

#endif