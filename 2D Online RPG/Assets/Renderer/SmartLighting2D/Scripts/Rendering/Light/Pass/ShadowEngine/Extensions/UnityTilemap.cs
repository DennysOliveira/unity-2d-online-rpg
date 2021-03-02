using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LightTilemapCollider;

namespace Rendering.Light.Shadow {

    public class UnityTilemap {

        static public void Draw(Light2D light, LightTilemapCollider2D id, float lightSizeSquared) {
            Vector2 lightPosition = -light.transform.position;
            LightTilemapCollider.Base tilemapCollider = id.GetCurrentTilemap();

            int count = tilemapCollider.chunkManager.GetTiles(light.GetWorldRect());

            for(int i = 0; i < count; i++) {
                LightingTile tile = tilemapCollider.chunkManager.display[i];

                //LightingTile tile
                switch(id.shadowTileType) {
                    case ShadowTileType.AllTiles:
                    break;

                    case ShadowTileType.ColliderOnly:
                        if (tile.colliderType == UnityEngine.Tilemaps.Tile.ColliderType.None) {
                            continue;
                        }
                    break;
                }

                List<Polygon2> polygons = tile.GetWorldPolygons(tilemapCollider);
                Vector2 tilePosition = tile.GetWorldPosition(tilemapCollider);

                if (tile.NotInRange(lightPosition + tilePosition, light.size)) {
                    continue;
                }

                ShadowEngine.Draw(polygons, 0,  0);
            }
        }
    }
}