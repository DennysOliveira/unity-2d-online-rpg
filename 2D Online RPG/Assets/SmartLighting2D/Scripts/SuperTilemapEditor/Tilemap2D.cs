using System.Collections.Generic;
using UnityEngine;
using LightTilemapCollider;

#if (SUPER_TILEMAP_EDITOR)

using CreativeSpore.SuperTilemapEditor;

    namespace SuperTilemapEditorSupport {

        public class TilemapCollider : LightTilemapCollider.Base {
            public List<Transform> colliderTransform = new List<Transform>();
            public List<Polygon2> localColliders = new List<Polygon2>();
            public List<Polygon2> worldColliders = new List<Polygon2>();

            public CreativeSpore.SuperTilemapEditor.STETilemap tilemap;
            public bool eventsInit = false;

            public enum ShadowType {None, Grid, TileCollider, Collider};
            public enum MaskType {None, Grid, Sprite, BumpedSprite};

            public ShadowType shadowTypeSTE = ShadowType.Grid;
            public MaskType maskTypeSTE = MaskType.Sprite;

            public override MapType TilemapType() {
                return MapType.SuperTilemapEditor;
            }

            private void OnTileChanged(STETilemap tilemap, int gridX, int gridY, uint tileData) {  
                Initialize();
			
				Light2D.ForceUpdateAll();
            }

            private void OnMeshUpdated(STETilemap source) {  
                Initialize();

                Light2D.ForceUpdateAll();
            }

            public override void Initialize() {
                base.Initialize();

                tilemap = gameObject.transform.GetComponent<STETilemap>();

                if (tilemap == null) {
                    return;
                }

                if (eventsInit == false) {
                    eventsInit = true;

                    tilemap.OnTileChanged += OnTileChanged;

                    tilemap.OnMeshUpdated += OnMeshUpdated;
                }
                
                properties.cellSize = tilemap.CellSize;

                InitializeGrid();

                InitializeColliders();

                chunkManager.Update(mapTiles, this);
            }

            public const uint k_TileFlag_FlipV = 0x80000000;
            public const uint k_TileFlag_FlipH = 0x40000000;
            
            public void InitializeGrid() {
                mapTiles.Clear();

                for (int x = tilemap.MinGridX; x <= tilemap.MaxGridX; x++) {
                    for (int y = tilemap.MinGridY; y <= tilemap.MaxGridY; y++) {
                        Tile tileSTE = tilemap.GetTile(x, y);
                        uint tileDataSTE = tilemap.GetTileData(x, y);
                        uint dataSTE = Tileset.GetTileFlagsFromTileData(tileDataSTE);

                        if (tileSTE == null) {
                            continue;
                        }

                        LightingTile tile = new LightingTile();
                        tile.gridPosition = new Vector3Int(x, y, 0);
                        tile.uv = tileSTE.uv;

                        bool flipX = (dataSTE & k_TileFlag_FlipH) != 0;
                        bool flipY = (dataSTE & k_TileFlag_FlipV) != 0;

                        Vector2 scale = Vector2.one;
                        if (flipX) {
                            scale.x = -1;
                        }

                        if (flipY) {
                            scale.y = - 1;
                        }

                        tile.scale = scale;

                        bool dynamic = this.shadowTypeSTE == SuperTilemapEditorSupport.TilemapCollider2D.ShadowType.TileCollider;
           
                        if (dynamic) {
                            List<Polygon2> polygons = TileColliderDataToPolygons(tileSTE.collData, tile.scale);

                            if (polygons.Count > 0) {
                                tile.SetLocalPolygons(polygons);
                            }
                            
                        }
                        
                        mapTiles.Add(tile);
                    }            
                }
            }

            List<Polygon2> TileColliderDataToPolygons(TileColliderData tileColliderData, Vector2 scale) {
                List<Polygon2> polygons = new List<Polygon2>();

                if (tileColliderData.vertices.Length > 0) {
                        
                    Polygon2 polygon = new Polygon2(tileColliderData.vertices.Length);

                    for(int i = 0; i < tileColliderData.vertices.Length; i++) {
                        Vector2 v = tileColliderData.vertices[i];

                        polygon.points[i] = v;
                    }

                    if (scale != Vector2.one) {
                        polygon.ToScaleSelf(scale);
                    }
                    
                    polygons.Add(polygon);

                }

                return(polygons);
            }
    
            public void InitializeColliders() {
                localColliders.Clear();
                worldColliders.Clear();
                colliderTransform.Clear();

                foreach(Transform transform in gameObject.transform) {
                    EdgeCollider2D[] edgeColliders = transform.GetComponents<EdgeCollider2D>();
                    PolygonCollider2D[] polygonColliders = transform.GetComponents<PolygonCollider2D>();

                    if (edgeColliders.Length > 0) {
                         foreach(EdgeCollider2D collider in edgeColliders) {
                            Polygon2 poly = Polygon2ListCollider2D.CreateFromEdgeCollider(collider);

                            localColliders.Add(poly);
                            colliderTransform.Add(transform);
                        }
                    }

                    if (polygonColliders.Length > 0) {
                        foreach(PolygonCollider2D collider in polygonColliders) {
                            Polygon2 poly = Polygon2ListCollider2D.CreateFromPolygonColliderToLocalSpace(collider)[0];
                    
                            localColliders.Add(poly);
                            colliderTransform.Add(transform);
                        }
                    }
                }
            }

            public List<Polygon2> GetWorldColliders() {
                worldColliders.Clear();

                for(int i = 0; i < localColliders.Count; i++) {
                    Polygon2 polygon = localColliders[i];
                    Transform transform = colliderTransform[i];

                    polygon = polygon.ToWorldSpace(transform);

                    worldColliders.Add(polygon);
                }

                return(worldColliders);
            }

            public override Vector2 TileWorldPosition(LightingTile tile) {
                Transform transform = Properties.transform;

                float rotation = transform.eulerAngles.z * Mathf.Deg2Rad;

                Vector2 resultPosition = transform.position;

                Vector2 tilePosition = new Vector2(tile.gridPosition.x, tile.gridPosition.y);

                tilePosition.x *= Properties.cellSize.x;
                tilePosition.y *= Properties.cellSize.y;

                tilePosition.x += Properties.cellAnchor.x * Properties.cellSize.x;
                tilePosition.y += Properties.cellAnchor.y * Properties.cellSize.y;

                tilePosition.x *= transform.lossyScale.x;
                tilePosition.y *= transform.lossyScale.y;

                // Rotation
                float tileDirection = Mathf.Atan2(tilePosition.y, tilePosition.x) + rotation;
                float length = Mathf.Sqrt(tilePosition.x * tilePosition.x + tilePosition.y * tilePosition.y);

                tilePosition.x = Mathf.Cos(tileDirection) * length;
                tilePosition.y = Mathf.Sin(tileDirection) * length;

                resultPosition += tilePosition;

                return(resultPosition);
            }

            public override float TileWorldRotation(LightingTile tile) {
                float worldRotation = tilemap.transform.eulerAngles.z;

                return(worldRotation);
            }

            public override Vector2 TileWorldScale() {
                Vector2 scale = Properties.transform.lossyScale;
                scale *= Properties.cellSize;
                return(scale);
            }
        }
    }

 #endif