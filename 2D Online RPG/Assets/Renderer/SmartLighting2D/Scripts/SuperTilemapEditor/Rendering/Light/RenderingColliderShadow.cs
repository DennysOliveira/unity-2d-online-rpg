using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LightTilemapCollider;

#if (SUPER_TILEMAP_EDITOR)

    namespace SuperTilemapEditorSupport {
        
        public class RenderingColliderShadow {

            static public void Grid(Light2D light, LightTilemapCollider2D id) {
                Vector2 lightPosition = -light.transform.position;
                LightTilemapCollider.Base tilemapCollider = id.GetCurrentTilemap();

                int count = tilemapCollider.chunkManager.GetTiles(light.GetWorldRect());

                for(int i = 0; i < count; i++) {
                    LightingTile tile = tilemapCollider.chunkManager.display[i];

                    List<Polygon2> polygons = tile.GetWorldPolygons(tilemapCollider);
                    Vector2 tilePosition = tile.GetWorldPosition(tilemapCollider);

                    if (tile.NotInRange(tilePosition + lightPosition, light.size)) {
                        continue;
                    }

                    Rendering.Light.ShadowEngine.Draw(polygons, 0, 0);
                }
            }

            static public void Collider(Light2D light, LightTilemapCollider2D id) {
                Rendering.Light.ShadowEngine.Draw(id.superTilemapEditor.GetWorldColliders(), 0, 0);
            }
        }
    }

#else 

    namespace SuperTilemapEditorSupport {
        public class RenderingColliderShadow { 
            static public void Grid(Light2D light, LightTilemapCollider2D id) {}
            static public void Collider(Light2D light, LightTilemapCollider2D id) {}
        }
    }

#endif