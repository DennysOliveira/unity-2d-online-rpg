using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LightingSettings;
using LightSettings;

namespace Rendering.Day {

    public class Pass {
        
        public Sorting.SortList sortList = new Sorting.SortList();
        public Sorting.SortObject sortObject;
        public int layerId;
        public LightingLayerSetting layer;

        public Camera camera;
        public Vector2 offset;

        public List<DayLightCollider2D> colliderList;
        public int colliderCount;

        public bool drawShadows = false;
        public bool drawMask = false;

        public List<DayLightTilemapCollider2D> tilemapColliderList;
        public int tilemapColliderCount;

        public void SortObjects() {
            sortList.Reset();

            List<DayLightCollider2D> colliderList = DayLightCollider2D.List;
            for(int id = 0; id < colliderList.Count; id++) {
                DayLightCollider2D collider = colliderList[id];

                if (collider.shadowLayer != layerId && collider.maskLayer != layerId) {
                    continue;
                }

                switch(layer.sorting) {
                    case LayerSorting.ZAxisLower:
                        sortList.Add((object)collider, Sorting.SortObject.Type.Collider, - collider.transform.position.z);
                    break;

                    case LayerSorting.ZAxisHigher:
                        sortList.Add((object)collider, Sorting.SortObject.Type.Collider, collider.transform.position.z);
                    break;
                }

                switch(layer.sorting) {
                    case LayerSorting.YAxisLower:
                        sortList.Add((object)collider, Sorting.SortObject.Type.Collider, - collider.transform.position.y);
                    break;

                    case LayerSorting.YAxisHigher:
                        sortList.Add((object)collider, Sorting.SortObject.Type.Collider, collider.transform.position.y);
                    break;
                }
            }

            List<DayLightTilemapCollider2D> tilemapColliderList = DayLightTilemapCollider2D.list;
            for(int id = 0; id < tilemapColliderList.Count; id++) {
                DayLightTilemapCollider2D tilemap = tilemapColliderList[id];

                if (tilemap.shadowLayer != layerId && tilemap.maskLayer != layerId) {
                    continue;
                }

                switch(layer.sorting) {
                    case LayerSorting.ZAxisLower:
                        sortList.Add((object)tilemap, Sorting.SortObject.Type.TilemapCollider, - tilemap.transform.position.z);
                    break;

                    case LayerSorting.ZAxisHigher:
                        sortList.Add((object)tilemap, Sorting.SortObject.Type.TilemapCollider, tilemap.transform.position.z);
                    break;
                }

                switch(layer.sorting) {
                    case LayerSorting.YAxisLower:
                        sortList.Add((object)tilemap, Sorting.SortObject.Type.TilemapCollider, - tilemap.transform.position.y);
                    break;

                    case LayerSorting.YAxisHigher:
                        sortList.Add((object)tilemap, Sorting.SortObject.Type.TilemapCollider, tilemap.transform.position.y);
                    break;
                }
            }

            sortList.Sort();
        }


        public bool Setup(LightingLayerSetting slayer, Camera camera) {
            if (slayer.layer < 0) {
                return(false);
            }

            layerId = (int)slayer.layer;
            layer = slayer;

            this.camera = camera;
            offset = -camera.transform.position;

            colliderList = DayLightCollider2D.List;
            colliderCount = colliderList.Count;

            tilemapColliderList  = DayLightTilemapCollider2D.GetList();
            tilemapColliderCount = tilemapColliderList.Count;

            drawShadows = slayer.type != LayerType.MaskOnly;
            drawMask = slayer.type != LayerType.ShadowsOnly;
            
            return(true);
        }
    }
}