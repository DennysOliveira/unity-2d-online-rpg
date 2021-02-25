using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LightingSettings;
using LightSettings;

namespace Rendering.Night {

    public class Pass {
        
        public Sorting.SortList sortList = new Sorting.SortList();
        public Sorting.SortObject sortObject;
        public int layerId;
        public LightingLayerSetting layer;

        public Camera camera;
        public Vector2 offset;

        public void SortObjects() {
            sortList.Reset();

            List<Light2D> lightList = Light2D.List;
            for(int id = 0; id < lightList.Count; id++) {
                Light2D light = lightList[id];

                if ((int)light.nightLayer != layerId) {
                    continue;
                }

                switch(layer.sorting) {
                    case LayerSorting.ZAxisLower:
                        sortList.Add((object)light, Sorting.SortObject.Type.Light, - light.transform.position.z);
                    break;

                    case LayerSorting.ZAxisHigher:
                        sortList.Add((object)light, Sorting.SortObject.Type.Light, light.transform.position.z);
                    break;
                }
            }

            List<LightRoom2D> roomList = LightRoom2D.List;
            for(int id = 0; id < roomList.Count; id++) {
                LightRoom2D room = roomList[id];

                if ((int)room.nightLayer != layerId) {
                    continue;
                }

                switch(layer.sorting) {

                    case LayerSorting.ZAxisLower:
                        sortList.Add((object)room, Sorting.SortObject.Type.Room, - room.transform.position.z);
                    break;

                    case LayerSorting.ZAxisHigher:
                        sortList.Add((object)room, Sorting.SortObject.Type.Room, room.transform.position.z);
                    break;
                }
            }

            List<LightTilemapRoom2D> roomTilemapList = LightTilemapRoom2D.List;
            for(int id = 0; id < roomTilemapList.Count; id++) {
                LightTilemapRoom2D tilemapRoom = roomTilemapList[id];

                if ((int)tilemapRoom.nightLayer != layerId) {
                    continue;
                }

                switch(layer.sorting) {

                    case LayerSorting.ZAxisLower:
                        sortList.Add((object)tilemapRoom, Sorting.SortObject.Type.TilemapRoom, - tilemapRoom.transform.position.z);
                    break;

                    case LayerSorting.ZAxisHigher:
                        sortList.Add((object)tilemapRoom, Sorting.SortObject.Type.TilemapRoom, tilemapRoom.transform.position.z);
                    break;
                }
            }

            List<LightSprite2D> spriteList = LightSprite2D.List;
            for(int id = 0; id < spriteList.Count; id++) {
                LightSprite2D lightSprite = spriteList[id];

                if ((int)lightSprite.nightLayer != layerId) {
                    continue;
                }

                switch(layer.sorting) {

                    case LayerSorting.ZAxisLower:
                        sortList.Add((object)lightSprite, Sorting.SortObject.Type.LightSprite, - lightSprite.transform.position.z);
                    break;

                    case LayerSorting.ZAxisHigher:
                        sortList.Add((object)lightSprite, Sorting.SortObject.Type.LightSprite, lightSprite.transform.position.z);
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

            return(true);
        }
    }
}