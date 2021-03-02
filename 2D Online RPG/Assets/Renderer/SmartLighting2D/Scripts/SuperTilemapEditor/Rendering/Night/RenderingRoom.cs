using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if (SUPER_TILEMAP_EDITOR)

    namespace SuperTilemapEditorSupport {

        public class RenderingRoom {

            public static void DrawTiles(Camera camera, LightTilemapRoom2D id, Material material) {
                Vector2 cameraPosition = -camera.transform.position;

                float cameraRadius = CameraTransform.GetRadius(camera);

                if (id.superTilemapEditor.tilemap == null) {
                    return;
                }

                if (id.superTilemapEditor.tilemap.Tileset != null) {
                    material.mainTexture = id.superTilemapEditor.tilemap.Tileset.AtlasTexture;
                }

                material.color = id.color;

                LightTilemapCollider.Base tilemapCollider = id.GetCurrentTilemap();

                material.SetPass (0); 
                GL.Begin (GL.QUADS);

                int count = id.superTilemapEditor.chunkManager.GetTiles( CameraTransform.GetWorldRect(camera) );

                for(int i = 0; i < count; i++) {
                    LightingTile tile = id.superTilemapEditor.chunkManager.display[i];

                    Vector2 tilePosition = tile.GetWorldPosition(tilemapCollider);
                    
                    tilePosition += cameraPosition;

                    if (tile.NotInRange(tilePosition, cameraRadius)) {
                        continue;
                    }

                    Vector2 scale = tile.worldScale * 0.5f * tile.scale;
                
                    Rendering.Universal.Texture.DrawPassSTE(tilePosition, scale, tile.uv, tile.worldRotation, 0);
                }

                GL.End ();

                material.mainTexture = null;
                material.color = Color.white;
            }
        }
    }

#else 

    namespace SuperTilemapEditorSupport {
        public class RenderingRoom {
            public static void DrawTiles(Camera camera, LightTilemapRoom2D id, Material materia) {}
        }
    }

#endif